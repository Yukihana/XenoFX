using System.Diagnostics.CodeAnalysis;

namespace CSX.Common.Data.Text;

public static class ArgumentExtensions
{
    public static bool TryGet(this ArgsArray args, string key, [NotNullWhen(true)] out string? value)
    {
        value = null;

        if (args == null || string.IsNullOrWhiteSpace(key))
            return false;

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == key)
            {
                value = args[i + 1];
                return true; // Found the key and its value
            }
        }

        return false;
    }
}