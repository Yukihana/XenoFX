namespace CSX.DotNet.Common.Logging;

public class LogPackage
{
    public string Message { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public object? Data { get; set; } = null;
}