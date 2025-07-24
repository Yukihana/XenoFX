using System;
using System.Security.Cryptography;

namespace CSX.Common.Data.Guids;

public static class DMC212710Guid
{
    private static readonly DateTime DefaultEpoch = new(
        year: 1900,
        month: 1,
        day: 1,
        hour: 0,
        minute: 0,
        second: 0,
        kind: DateTimeKind.Utc);

    private const int MaxMsPerDay = 24 * 60 * 60 * 1000;

    /// <summary>
    /// This UID scheme embeds a 48-bit timestamp
    /// (21 bits for total days, 27 bits for millisecond-precision time of day),
    /// followed by 80 bits (10 bytes) of cryptographically secure randomness.
    /// The probability of a collision within the same millisecond is at most 1 in 2^80,
    /// assuming a cryptographically secure random number generator.
    /// This approach maintains temporal order while ensuring
    /// high uniqueness in distributed or high-throughput systems.
    /// </summary>
    public static Guid NewGuid(
        DateTime dateTime,
        DateTime? epoch = null)
    {
        epoch ??= DefaultEpoch;
        TimeSpan sinceEpoch = dateTime - epoch.Value;

        // Total days (21 bits)
        int totalDays = (int)(sinceEpoch.TotalDays % (1 << 21));

        // Milliseconds since start of day (27 bits)
        // Prevent overflow for floor based rounding
        // Not actually needed, but for edge case safety
        int msOfDay = Math.Min(
            (int)dateTime.TimeOfDay.TotalMilliseconds,
            MaxMsPerDay - 1);

        // Pack 21 bits of days + 27 bits of ms into 6 bytes (usable: 48 out of 64 bits)
        ulong timestamp = (ulong)totalDays << 27 | (uint)msOfDay;

        // Copy the timestamp into the first 6 bytes (big-endian)
        byte[] guidBytes = new byte[16];
        for (int i = 0; i < 6; i++)
            guidBytes[i] = (byte)(timestamp >> 40 - i * 8);

        // Copy 10 bytes of cryptographically secure RNG
        RandomNumberGenerator.Fill(guidBytes.AsSpan(6));

        return new Guid(guidBytes);
    }

    public static Guid FromUtcNow(DateTime? epoch = null)
    {
        DateTime dateTime = DateTime.UtcNow;
        return NewGuid(dateTime, epoch);
    }
}