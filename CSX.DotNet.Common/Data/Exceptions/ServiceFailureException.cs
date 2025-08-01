using System;

namespace CSX.DotNet.Common.Data.Exceptions;

public class ServiceFailureException : Exception
{
    public ServiceFailureException() : base()
    { }

    public ServiceFailureException(string? message) : base(message)
    { }
}