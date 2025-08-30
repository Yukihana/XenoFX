using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace CSX.DotNet.Common.EFC.Columns;

public static class GuidByteBuilder
{
    // Column type

    private static readonly FrozenDictionary<string, string> _providerTypeDictionary = new Dictionary<string, string>()
    {
        ["Npgsql.EntityFrameworkCore.PostgreSQL"] = "BYTEA",
        ["Pomelo.EntityFrameworkCore.MySql"] = "BINARY(16)",
        ["Microsoft.EntityFrameworkCore.Sqlite"] = "BLOB",
        ["Oracle.EntityFrameworkCore"] = "RAW(16)"
    }.ToFrozenDictionary(); // Ensures faster operations on .NET7+

    public static IReadOnlyDictionary<string, string> ProviderTypeDictionary
        => _providerTypeDictionary;

    public static string GetColumnType(string dbProvider)
    {
        return ProviderTypeDictionary.TryGetValue(dbProvider, out var type)
            ? type
            : "BINARY(16)"; // Default
    }

    // Converter

    private static readonly ValueConverter<Guid, byte[]> _converter = new(
        guid => guid.ToByteArray(),
        bytes => new Guid(bytes));

    public static ValueConverter<Guid, byte[]> Converter
        => _converter;
}