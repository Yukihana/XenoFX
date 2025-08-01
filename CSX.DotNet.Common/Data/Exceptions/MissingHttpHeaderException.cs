using System;

namespace CSX.DotNet.Common.Data.Exceptions;

public class MissingHttpHeaderException : Exception
{
    public string HeaderName { get; }

    // Lifecycle

    public MissingHttpHeaderException(string headerName)
        : base($"Missing or invalid header: {headerName}")
        => HeaderName = headerName;
}