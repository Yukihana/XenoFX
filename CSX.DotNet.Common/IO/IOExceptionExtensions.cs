using System.IO;

namespace CSX.DotNet.Common.IO;

public static class IOExceptionExtensions
{
    public const int ERROR_FILE_EXISTS = 0x50;           // 80
    public const int ERROR_ALREADY_EXISTS = 0xB7;         // 183

    public static bool IsFileAlreadyExists(
        this IOException ex)
    {
        // Get Win32 error code
        int hresult = ex.HResult & 0xFFFF;

        return
            hresult == ERROR_FILE_EXISTS ||
            hresult == ERROR_ALREADY_EXISTS;
    }
}