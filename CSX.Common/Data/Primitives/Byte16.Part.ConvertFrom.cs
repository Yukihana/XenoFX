using System;
using System.Globalization;
using System.Linq;

namespace CSX.Common.Data.Primitives;

public readonly partial struct Byte16
{
    // Presets

    public static Byte16 Zero() => new(0, 0);

    // Factory

    public static Byte16 FromBytes(byte[] bytes)
    {
        if (bytes == null || bytes.Length != 16)
            throw new ArgumentException("Byte array must be exactly 16 bytes long.", nameof(bytes));

        ulong high = BitConverter.ToUInt64(bytes, 0);
        ulong low = BitConverter.ToUInt64(bytes, 8);
        return new(high, low);
    }

    public static Byte16 FromUShorts(ushort[] shorts)
    {
        if (shorts.Length != 8)
            throw new ArgumentException("Array must be exactly 8 elements long.", nameof(shorts));

        byte[] bytes = shorts.SelectMany(BitConverter.GetBytes).ToArray();
        ulong high = BitConverter.ToUInt64(bytes, 0);
        ulong low = BitConverter.ToUInt64(bytes, 8);
        return new(high, low);
    }

    public static Byte16 FromUInts(uint[] ints)
    {
        if (ints == null || ints.Length != 4)
            throw new ArgumentException("Integer array must be exactly 4 elements long.", nameof(ints));

        byte[] bytes = ints.SelectMany(BitConverter.GetBytes).ToArray();
        ulong high = BitConverter.ToUInt64(bytes, 0);
        ulong low = BitConverter.ToUInt64(bytes, 8);
        return new(high, low);
    }

    public static Byte16 FromGuid(Guid guid)
        => FromBytes(guid.ToByteArray());

    public static Byte16 FromString(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Input cannot be null or empty.", nameof(text));

        // Allocate a fixed-size buffer (since we expect 32 hex chars max)
        Span<char> buffer = stackalloc char[32];
        int count = 0;

        // Filter only hex characters efficiently into the buffer
        foreach (char c in text)
        {
            if (count == 32) // More than 32 hex digits = invalid
                throw new FormatException("Invalid input length. Expected 32 hex characters.");
            if (Uri.IsHexDigit(c))
            {
                buffer[count++] = c;
            }
            else if (!"-_ .:;".Contains(c))
            {
                throw new FormatException($"Invalid character '{c}' in input.");
            }
        }

        // Ensure we got exactly 32 hex characters
        if (count != 32)
            throw new FormatException("Invalid input length. Expected 32 hex characters.");

        // Convert hex chars to bytes
        Span<byte> bytes = stackalloc byte[16];
        for (int i = 0; i < 16; i++)
            bytes[i] = byte.Parse(buffer.Slice(i * 2, 2), NumberStyles.HexNumber);

        // Convert to two ulongs
        ulong high = BitConverter.ToUInt64(bytes.Slice(0, 8));
        ulong low = BitConverter.ToUInt64(bytes.Slice(8, 8));

        return new Byte16(high, low);
    }
}