using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Google.Protobuf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace PaperBoy.Core.ProtoParsing.Dynamic;

public static class ProtoToClrTypesCompiler
{
    public static Assembly Compile(string folder)
    {
        string[] csFiles = Directory.GetFiles(folder, "*.cs");

        IEnumerable<SyntaxTree> syntaxTrees = csFiles
            .Select(path => CSharpSyntaxTree.ParseText(File.ReadAllText(path)));

        // https://learn.microsoft.com/en-us/dotnet/core/dependency-loading/default-probing
        string[] trustedAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!
            .ToString()!
            .Split(Path.PathSeparator);

        List<PortableExecutableReference> references = trustedAssemblies
            .Select(p => MetadataReference.CreateFromFile(p))
            .ToList();

        references.Add(
            MetadataReference.CreateFromFile(typeof(IMessage).Assembly.Location));

        var compilation = CSharpCompilation.Create(
            "ProtoDynamic",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        EmitResult result = compilation.Emit(ms);

        if (!result.Success)
        {
            throw new Exception(string.Join(Environment.NewLine, result.Diagnostics));
        }

        ms.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(ms.ToArray());
    }

    public static IMessage CreateMessage(Assembly assembly, string typeFullName)
    {
        Type type = assembly.GetTypes()
            .FirstOrDefault(t =>
                t.FullName == typeFullName &&
                typeof(IMessage).IsAssignableFrom(t) &&
                t.IsAbstract is false &&
                t.IsInterface is false);

        if (type == null)
        {
            throw new Exception($"No protobuf message type found for TypeName: {typeFullName}");
        }

        return (IMessage)Activator.CreateInstance(type);
    }

    public static IEnumerable<IMessage> CreateMessages(Assembly assembly)
    {
        IEnumerable<Type> types = assembly.GetTypes()
            .Where(t =>
                typeof(IMessage).IsAssignableFrom(t) &&
                t.IsAbstract is false &&
                t.IsInterface is false);

        foreach (Type type in types)
        {
            yield return (IMessage)Activator.CreateInstance(type);
        }
    }
}