using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace XenoFx.Database.ValueConverters;

public static partial class UInt128Converters
{
    public static ValueConverter<UInt128, byte[]> UInt128ByteConverter { get; } = new(
        v => BitConverter.GetBytes(v),
        v => BitConverter.ToUInt128(v));
}