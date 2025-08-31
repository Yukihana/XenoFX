using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.Platform.Processes;

public static partial class ProcessExecution
{
    public static async Task RunProcessAsync(
        string binaryPath,
        string arguments,
        CancellationToken ctoken = default)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = binaryPath,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        process.Start();

        try
        {
            await process.WaitForExitAsync(ctoken);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
            {
                try { process.Kill(true); } catch { }
            }
            throw;
        }

        if (process.ExitCode != 0)
            throw new Exception($"Process exited with code {process.ExitCode}");
    }

    public static async Task<string> RunAndReadAsync(
        string binaryPath,
        string arguments,
        CancellationToken ctoken = default)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = binaryPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        string output = (await process.StandardOutput.ReadToEndAsync(ctoken)).Trim();
        await process.WaitForExitAsync(ctoken);
        return output;
    }
}