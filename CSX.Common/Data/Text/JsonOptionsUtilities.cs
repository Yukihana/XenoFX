using System.Text.Json;

namespace CSX.Common.Data.Text;

public static class JsonOptionsUtilities
{
    public static JsonSerializerOptions HumanReadableJsonOptions => new()
    {
        WriteIndented = true
    };
}