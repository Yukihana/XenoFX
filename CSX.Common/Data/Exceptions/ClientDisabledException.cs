using System;

namespace CSX.Common.Data.Exceptions;

public class ClientDisabledException : Exception
{
    public ClientDisabledException() : base()
    { }

    public ClientDisabledException(string? message) : base(message)
    { }
}