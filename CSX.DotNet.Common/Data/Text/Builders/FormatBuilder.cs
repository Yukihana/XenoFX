using System.Collections.Generic;

namespace CSX.DotNet.Common.Data.Text.Builders;

public class FormatBuilder
{
    // Data

    protected readonly Dictionary<string, string> _components = [];
    protected string _format;

    // Public parameters

    public string FormatlessSeparator { get; set; } = "_";

    // Lifecycle

    protected FormatBuilder(string format)
        => _format = format;

    // Enforced factory pattern

    public static FormatBuilder Create(
        string format = "",
        IDictionary<string, string>? components = null)
    {
        var formatBuilder = new FormatBuilder(format);

        if (components != null)
        {
            foreach (var kv in components)
                formatBuilder.SetComponent(kv.Key, kv.Value);
        }

        return formatBuilder;
    }

    public static FormatBuilder Create(
        string format,
        params (string Key, string Value)[] components)
    {
        var formatBuilder = new FormatBuilder(format);

        foreach (var (key, value) in components)
            formatBuilder.SetComponent(key, value);

        return formatBuilder;
    }

    // Output

    public override string ToString()
    {
        // If without format, return components joined by separator
        if (string.IsNullOrWhiteSpace(_format))
            return string.Join(FormatlessSeparator, _components.Values);

        // If format is provided, replace components in the format string
        string result = _format;
        foreach (var kv in _components)
            result = result.Replace($"{{{kv.Key}}}", kv.Value);

        return result;
    }

    // Public API

    public string this[string key]
    {
        get => _components[key];
        set => _components[key] = value;
    }

    // Fluent

    public virtual FormatBuilder SetComponent(string key, string value)
    {
        _components[key] = value;
        return this;
    }
}