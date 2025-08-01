using System;

namespace CSX.DotNet.Common.Data.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException() : base()
    {
    }

    public BadRequestException(string message) : base(message)
    {
    }
}