using System;

namespace CSX.DotNet.Common.Data.Exceptions;

public class ActionExecutionException : Exception
{
    public ActionExecutionException()
    {
    }

    public ActionExecutionException(
        string? message)
        : base(message)
    {
    }

    public ActionExecutionException(
        string? message,
        Exception? innerException)
        : base(message, innerException)
    {
    }

    public ActionExecutionException(
        Exception innerException)
        : base("Action execution has encountered an exception.", innerException)
    {
    }
}