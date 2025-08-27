using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Common.Platform.Unix;

public static class UnixExecutableUtility
{
    public static async Task SetUnixExecuteBitAsync(
        string executablePath,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Ensure the platform is Unix-like
        if (OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("This method is not supported on Windows.");

        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath, nameof(executablePath));

        if (!File.Exists(executablePath))
            throw new FileNotFoundException($"The specified file does not exist: {executablePath}");

        var psi = new ProcessStartInfo
        {
            FileName = "chmod",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        psi.ArgumentList.Add("+x");
        psi.ArgumentList.Add(executablePath);

        using var chmod = Process.Start(psi);

        if (chmod != null)
        {
            await chmod.WaitForExitAsync(ctoken);
            if (chmod.ExitCode != 0)
            {
                string error = await chmod.StandardError.ReadToEndAsync(ctoken);
                throw new InvalidOperationException($"chmod failed: {error}");
            }
        }
    }
}