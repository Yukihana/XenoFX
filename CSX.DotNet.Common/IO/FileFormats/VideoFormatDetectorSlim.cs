using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CSX.DotNet.Common.IO.FileFormats;

public static class VideoFormatDetectorSlim
{
    // Wrapper: Match against list of extensions
    public static string MatchExtensionFast(
        Stream stream,
        IEnumerable<string> extensions)
    {
        foreach (var ext in extensions)
        {
            if (IsFormat(stream, ext))
                return ext;
        }

        return string.Empty;
    }

    // Dispatcher: Calls the relevant format validator
    public static bool IsFormat(Stream stream, string ext)
    {
        ext = ext.TrimStart('.').ToLowerInvariant();
        return ext switch
        {
            "mp4" => IsMp4(stream),
            "webm" => IsWebM(stream),
            "mkv" => IsMkv(stream),
            "flv" => IsFlv(stream),
            "3gp" => Is3gp(stream),
            _ => false
        };
    }

    // --- Format Checkers Below ---
    private static bool IsMp4(Stream stream)
    {
        byte[] buffer = new byte[12];
        if (!TryRead(stream, buffer)) return false;

        // Check for ftyp box at offset 4
        return buffer[4] == (byte)'f' &&
               buffer[5] == (byte)'t' &&
               buffer[6] == (byte)'y' &&
               buffer[7] == (byte)'p';
    }

    private static bool IsWebM(Stream stream)
    {
        byte[] buffer = new byte[4];
        if (!TryRead(stream, buffer)) return false;

        // WebM/MKV: EBML magic number
        return buffer[0] == 0x1A && buffer[1] == 0x45 &&
               buffer[2] == 0xDF && buffer[3] == 0xA3;
    }

    private static bool IsMkv(Stream stream)
        => IsWebM(stream); // Shared header

    private static bool IsFlv(Stream stream)
    {
        byte[] buffer = new byte[3];
        if (!TryRead(stream, buffer)) return false;

        return buffer[0] == (byte)'F' &&
               buffer[1] == (byte)'L' &&
               buffer[2] == (byte)'V';
    }

    private static bool Is3gp(Stream stream)
    {
        byte[] buffer = new byte[12];
        if (!TryRead(stream, buffer)) return false;

        return buffer[4] == 0x66 && buffer[5] == 0x74 &&
           buffer[6] == 0x79 && buffer[7] == 0x70 &&
           buffer.AsSpan(8, 3).SequenceEqual("3gp"u8);
    }

    // Helper to safely read and rewind stream
    private static bool TryRead(Stream stream, byte[] buffer)
    {
        long pos = stream.Position;
        try
        {
            int read = stream.Read(buffer, 0, buffer.Length);
            return read == buffer.Length;
        }
        finally
        {
            stream.Position = pos;
        }
    }

    // Older
    [Obsolete("Use MatchExtensionFast instead")]
    public static string? DetectVideoFormat(Stream stream)
    {
        if (stream == null || !stream.CanRead)
            return null;

        byte[] buffer = new byte[64];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        if (bytesRead < 12)
            return null;

        // Rewind for re-use
        stream.Seek(0, SeekOrigin.Begin);

        // FLV: 46 4C 56
        if (buffer[0] == 0x46 && buffer[1] == 0x4C && buffer[2] == 0x56)
            return "flv";

        // MKV/WebM: EBML header = 1A 45 DF A3
        if (buffer[0] == 0x1A && buffer[1] == 0x45 && buffer[2] == 0xDF && buffer[3] == 0xA3)
        {
            string ascii = Encoding.ASCII.GetString(buffer);
            if (ascii.Contains("webm"))
                return "webm";
            return "mkv";
        }

        // MP4/3GP: ftyp at byte 4
        if (buffer[4] == 0x66 && buffer[5] == 0x74 && buffer[6] == 0x79 && buffer[7] == 0x70)
        {
            string brand = Encoding.ASCII.GetString(buffer.Skip(8).Take(4).ToArray());
            if (brand.StartsWith("3gp", StringComparison.OrdinalIgnoreCase))
                return "3gp";
            return "mp4";
        }

        return null; // Unknown or unsupported
    }
}