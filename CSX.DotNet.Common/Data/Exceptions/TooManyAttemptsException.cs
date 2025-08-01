using System;

namespace CSX.DotNet.Common.Data.Exceptions;

public class TooManyAttemptsException : Exception
{
    public TooManyAttemptsException() : base()
    {
    }

    public TooManyAttemptsException(string? message) : base(message)
    {
    }
}