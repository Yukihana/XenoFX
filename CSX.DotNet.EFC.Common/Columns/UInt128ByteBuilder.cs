using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace CSX.DotNet.EFC.Common.Columns;

public static partial class UInt128ByteBuilder
{
    public static ValueConverter<UInt128, byte[]> Converter { get; } = new(
        v => BitConverter.GetBytes(v),
        v => BitConverter.ToUInt128(v));
}