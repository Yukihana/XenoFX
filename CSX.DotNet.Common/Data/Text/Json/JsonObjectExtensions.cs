using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CSX.DotNet.Common.Data.Text.Json;

public static partial class JsonObjectExtensions
{
    public static JsonObject AttachPropertyWithCamelCasedName<T>(
        this JsonObject jsonObject,
        string propertyName,
        T? value)
    {
        // Guard
        ArgumentNullException.ThrowIfNull(jsonObject);

        if (string.IsNullOrWhiteSpace(propertyName))
            throw new ArgumentException("Property name cannot be null or whitespace.", nameof(propertyName));

        // Skip nulls and empty strings
        if (value == null || value is string s && string.IsNullOrWhiteSpace(s))
            return jsonObject;

        string camelCased = JsonNamingPolicy.CamelCase.ConvertName(propertyName);
        jsonObject[camelCased] = JsonSerializer.SerializeToNode(value);
        return jsonObject;
    }
}