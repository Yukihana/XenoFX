using System;

namespace CSX.Common.Data.Exceptions;

public class UnsupportedFileTypeException(
    string? message = null,
    string type = "")
    : Exception(message)
{
    public string Type { get; } = type;
}