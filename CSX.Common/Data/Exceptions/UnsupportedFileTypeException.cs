using System;

namespace CSX.Common.Data.Exceptions;

public class UnsupportedFileTypeException(
    string message,
    string type = "")
    : Exception(message)
{
    public string Type { get; } = type;
}