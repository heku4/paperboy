using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PaperBoy.Core.ProtoParsing;

public class ProtocDescriptorExecutor
{
    private readonly ILogger<ProtocDescriptorExecutor> _logger;

    public ProtocDescriptorExecutor(ILogger<ProtocDescriptorExecutor> logger)
    {
        _logger = logger;
    }

    public async Task<int> RunProtocAsync(
        string protoFile,
        string protoIncludePath,
        CancellationToken cancellationToken)
    {
        string descriptorFileName = GetDescriptorFileName(protoFile);
        string importPath = string.IsNullOrWhiteSpace(protoIncludePath) ? string.Empty : $"-I=\"{protoIncludePath}\" ";

        var startInfo = new ProcessStartInfo
        {
            FileName = "protoc",
            Arguments = $"--descriptor_set_out=\"{descriptorFileName}\" --include_imports {importPath} {protoFile}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = startInfo;

        process.Start();

        string stdout = await process.StandardOutput.ReadToEndAsync(cancellationToken);
        string stderr = await process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        if (_logger.IsEnabled(LogLevel.Debug) && string.IsNullOrWhiteSpace(stdout) is false)
        {
            _logger.LogDebug("protoc output:\n" + stdout);
        }

        if (process.ExitCode != 0)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("protoc err output:\n" + stderr);
            }

            throw new Exception($"protoc failed with exit code: {process.ExitCode}");
        }

        return process.ExitCode;
    }

    private static string GetDescriptorFileName(string protoFile)
    {
        string descriptorFileName = protoFile.Split('.')[0] + ".desc";
        return descriptorFileName;
    }

    public async Task<byte[]> GetDescriptorFileData(
        string protoFile,
        CancellationToken cancellationToken)
    {
        string descriptorFileName = GetDescriptorFileName(protoFile);
        return await File.ReadAllBytesAsync(descriptorFileName, cancellationToken);
    }
}