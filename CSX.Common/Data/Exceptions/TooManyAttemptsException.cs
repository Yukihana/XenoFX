using System;

namespace CSX.Common.Data.Exceptions;

public class TooManyAttemptsException : Exception
{
    public TooManyAttemptsException() : base()
    {
    }

    public TooManyAttemptsException(string? message) : base(message)
    {
    }
}