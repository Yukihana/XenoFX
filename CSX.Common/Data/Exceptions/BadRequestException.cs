using System;

namespace CSX.Common.Data.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException() : base()
    {
    }

    public BadRequestException(string message) : base(message)
    {
    }
}