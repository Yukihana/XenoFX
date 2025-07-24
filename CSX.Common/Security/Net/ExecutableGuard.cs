using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSX.Common.Security.Net;

public static class ExecutableGuard
{
    private static readonly HashSet<string> ExecutableMimeTypes = [
        "application/x-msdownload",
        "application/x-executable",
        "application/x-msdos-program",
        "application/x-dosexec",
        "application/x-sharedlib"];

    private static readonly HashSet<string> ExecutableExtensions
        = ["exe", "bat", "cmd", "com", "msi", "pif", "scr", "cpl"];

    // API : Mime

    public static bool IsExecutableMimeType(
        string mimeType)
    {
        return ExecutableMimeTypes.Contains(mimeType, StringComparer.OrdinalIgnoreCase);
    }

    // API : Filename

    public static bool IsExecutableFilename(
        string filename)
    {
        string fileExtension = Path
            .GetExtension(filename);
        return IsExecutableExtension(fileExtension);
    }

    public static bool IsExecutableExtension(
        string extension)
    {
        return ExecutableExtensions.Contains(
            extension.Trim().TrimStart('.'),
            StringComparer.OrdinalIgnoreCase);
    }

    // API : Content

    public static bool IsExecutable(string pathToCacheFile)
    {
        using FileStream fs = File.OpenRead(pathToCacheFile);
        return IsExecutable(fs);
    }

    public static bool IsExecutable(Stream stream)
    {
        if (stream == null || !stream.CanRead)
            return false;

        const int headerSize = 4; // Max needed for signature checks
        Span<byte> headerBytes = stackalloc byte[headerSize];

        long originalPosition = 0;
        bool restorePosition = stream.CanSeek;

        try
        {
            if (restorePosition)
                originalPosition = stream.Position;

            int bytesRead = stream.Read(headerBytes);

            if (bytesRead < headerSize)
                return false;

            return IsExecutableHeader(headerBytes.ToArray());
        }
        finally
        {
            if (restorePosition)
                stream.Position = originalPosition;
        }
    }

    public static bool IsExecutableHeader(ReadOnlySpan<byte> headerBytes)
    {
        if (headerBytes.Length < 4)
            return false;

        // Windows PE
        if (headerBytes[0] == 0x4D &&
            headerBytes[1] == 0x5A)
            return true;

        // ELF (Unix/Linux)
        if (headerBytes[0] == 0x7F &&
            headerBytes[1] == 0x45 &&
            headerBytes[2] == 0x4C &&
            headerBytes[3] == 0x46)
            return true;

        // Mach-O (macOS, 32/64/FAT)
        uint[] endianReversible = [
            0xFEEDFACE,     // MacOS 32bit
            0xFEEDFACF,     // MacOS 64bit
            0xCAFEBABE];    // FAT Binary

        // Compare each value both ways to compensate for endian-ness
        if (endianReversible.Contains(BinaryPrimitives.ReadUInt32BigEndian(headerBytes)) ||
            endianReversible.Contains(BinaryPrimitives.ReadUInt32LittleEndian(headerBytes)))
            return true;

        // Shebang for script execution
        if (headerBytes[0] == (byte)'#' && headerBytes[1] == (byte)'!')
            return true;

        return false;
    }
}