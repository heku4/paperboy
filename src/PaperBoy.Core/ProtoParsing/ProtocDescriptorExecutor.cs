using System;
using System.Diagnostics;
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
        string outputDescriptor,
        string protoIncludePath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "protoc",
            Arguments = $"--descriptor_set_out=\"{outputDescriptor}\" " +
                        $"--include_imports " +
                        $"-I=\"{protoIncludePath}\" " +
                        $"\"{protoFile}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = startInfo;

        process.Start();

        string stdout = await process.StandardOutput.ReadToEndAsync();
        string stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

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
}