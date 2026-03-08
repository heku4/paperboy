using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.Extensions.Logging;
using PaperBoy.Core.ProtoParsing.Dynamic;

namespace PaperBoy.Core.ProtoParsing;

public class ProtoParser : IProtoParser
{
    private readonly JsonParser _jsonParser;
    private readonly ILogger<ProtoParser> _logger;
    private readonly ProtocDescriptorExecutor _protocDescriptorExecutor;

    public ProtoParser(
        JsonParser jsonParser,
        ProtocDescriptorExecutor protocDescriptorExecutor,
        ILogger<ProtoParser> logger)
    {
        _jsonParser = jsonParser;
        _protocDescriptorExecutor = protocDescriptorExecutor;
        _logger = logger;
    }


    public async Task<string> ParseToJsonWithStub(
        Stream originalProtoFile,
        string fileName,
        CancellationToken cancellationToken)
    {
        try
        {
            byte[] descriptorFileData =
                await GetDescriptorFileFromProto(originalProtoFile, fileName, cancellationToken);
            return _jsonParser.ParseToJsonWithStub(descriptorFileData);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(e.StackTrace);
            }
        }

        return string.Empty;
    }

    public async Task<string> ParseToJsonWithStubDynamic(
        Stream originalProtoFile,
        string fileName,
        CancellationToken cancellationToken)
    {
        try
        {
            _ = await GetDescriptorFileFromProto(originalProtoFile, fileName, cancellationToken);
            Assembly assembly = ProtoToClrTypesCompiler.Compile("./generated");
            IEnumerable<IMessage> protoMessages = ProtoToClrTypesCompiler.CreateMessages(assembly);
            IEnumerable<string> serializedMessages =
                protoMessages.Select(m => _jsonParser.SerializeProtoFromClrMessage(m));

            return string.Join(Environment.NewLine, serializedMessages);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(e.StackTrace);
            }
        }

        return string.Empty;
    }

    public async Task<string> ParseToJson(
        Stream originalProtoFile,
        string fileName,
        CancellationToken cancellationToken)
    {
        byte[] descriptorFileData = await GetDescriptorFileFromProto(originalProtoFile, fileName, cancellationToken);
        FileDescriptorSet descriptorSet = FileDescriptorSet.Parser.ParseFrom(descriptorFileData);

        return _jsonParser.ParseWithDefaultValues(descriptorSet);
    }

    private async Task<byte[]> GetDescriptorFileFromProto(
        Stream file,
        string fileName,
        CancellationToken cancellationToken)
    {
        await SaveProtoFile(fileName, file, cancellationToken);

        int exitCode = await _protocDescriptorExecutor.RunProtocAsync(fileName, null, cancellationToken);
        if (exitCode != 0)
        {
            throw new Exception($"Protoc exited with code {exitCode}");
        }

        return await _protocDescriptorExecutor.GetDescriptorFileData(fileName, cancellationToken);
    }

    private async Task SaveProtoFile(string fileName, Stream descriptorFileData, CancellationToken cancellationToken)
    {
        using var sr = new MemoryStream();
        await descriptorFileData.CopyToAsync(sr, cancellationToken);
        await File.WriteAllBytesAsync(fileName, sr.ToArray(), cancellationToken);
    }
}