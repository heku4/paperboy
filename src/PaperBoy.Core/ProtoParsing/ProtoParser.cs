using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.Extensions.Logging;

namespace PaperBoy.Core.ProtoParsing;

public partial class ProtoParser : IProtoParser
{
    private readonly ILogger<ProtoParser> _logger;

    public ProtoParser(ILogger<ProtoParser> logger)
    {
        _logger = logger;
    }

    public string ParseToJsonWithStub(byte[] protoDescription)
    {
        FileDescriptorSet fileDescriptorSet = FileDescriptorSet.Parser.ParseFrom(protoDescription);

        IReadOnlyList<FileDescriptor> fileDescriptor =
            FileDescriptor.BuildFromByteStrings(fileDescriptorSet.File.Select(x => x.ToByteString()));

        StringBuilder sb = new StringBuilder().Append('{');
        foreach (FileDescriptor descriptor in fileDescriptor)
        {
            foreach (ServiceDescriptor service in descriptor.Services)
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    LogServiceServiceFullName(service.FullName);
                }

                foreach (MethodDescriptor method in service.Methods)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        LogMethodMethodName(method.Name);
                        LogRequestTypeInputTypeFullname(method.InputType.FullName);
                    }

                    IMessage requestMessage = CreateEmptyMessage(method.InputType);
                    sb.Append(JsonFormatter.Default.Format(requestMessage));
                }
            }
        }

        return sb.Append('}').ToString();
    }

    private static IMessage CreateEmptyMessage(MessageDescriptor descriptor)
    {
        var message = (IMessage)Activator.CreateInstance(descriptor.ClrType);

        foreach (FieldDescriptor field in descriptor.Fields.InDeclarationOrder())
        {
            if (field.Accessor != null && !field.IsRepeated)
            {
                field.Accessor.SetValue(message, GetDefaultValue(field));
            }
        }

        return message;
    }

    private static object GetDefaultValue(FieldDescriptor field)
    {
        return field.FieldType switch
        {
            FieldType.String => "",
            FieldType.Int32 => 0,
            FieldType.Int64 => 0L,
            FieldType.Bool => false,
            FieldType.Double => 0.0,
            FieldType.Float => 0f,
            FieldType.Enum => field.EnumType.Values.First().Name,
            FieldType.Message => MakeProtoMessageStub(field),
            _ => null
        };
    }

    private static object MakeProtoMessageStub(FieldDescriptor field)
    {
        return GetDefaultValue(field);
    }

    [LoggerMessage(LogLevel.Debug, "Method: {methodName}")]
    partial void LogMethodMethodName(string methodName);

    [LoggerMessage(LogLevel.Debug, "Service: {serviceFullName}")]
    partial void LogServiceServiceFullName(string serviceFullName);

    [LoggerMessage(LogLevel.Debug, "Request Type: {inputTypeFullName}")]
    partial void LogRequestTypeInputTypeFullname(string inputTypeFullName);
}