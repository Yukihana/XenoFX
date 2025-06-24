using System;

namespace CSX.Common.Data.Exceptions;

public class ServiceFailureException : Exception
{
    public ServiceFailureException() : base()
    { }

    public ServiceFailureException(string? message) : base(message)
    { }
}