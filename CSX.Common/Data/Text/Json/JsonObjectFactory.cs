using System.Text.Json;
using System.Text.Json.Nodes;

namespace CSX.Common.Data.Text.Json;

public static partial class JsonObjectFactory
{
    public const string DefaultRawPropertyName = "default";

    public static JsonObject NormalizeToJsonObject<T>(
        T rawData)
    {
        // Null input: return empty object
        if (rawData == null)
            return [];

        // Already a JsonObject: return as-is
        if (rawData is JsonObject obj)
            return obj;

        // If it's a string, try to parse as JSON
        if (rawData is string rawString)
        {
            try
            {
                var parsedNode = JsonNode.Parse(rawString);
                if (parsedNode is JsonObject parsedObj)
                    return parsedObj;

                // If parsed but not a JsonObject (e.g., array, value), wrap it
                return new JsonObject { [DefaultRawPropertyName] = parsedNode };
            }
            catch
            {
                // Parsing failed, treat as plain string
                return new JsonObject { [DefaultRawPropertyName] = rawString };
            }
        }

        // Other types: try to serialize
        try
        {
            JsonNode? node = JsonSerializer.SerializeToNode(rawData);

            // If it's a JsonObject, return directly
            if (node is JsonObject objectNode)
                return objectNode;

            // If not a JsonObject (e.g., string, primitive, array), wrap it
            return new JsonObject { [DefaultRawPropertyName] = node };
        }
        catch
        {
            // Fallback to ToString() if serialization fails
            return new JsonObject
            {
                [DefaultRawPropertyName] = rawData?.ToString() ?? string.Empty
            };
        }
    }
}