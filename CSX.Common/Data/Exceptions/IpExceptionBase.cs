using System;
using System.Net;

namespace CSX.Common.Data.Exceptions;

public abstract class IpExceptionBase : Exception
{
    public IpExceptionBase()
    { }

    public IpExceptionBase(string? message) : base(message)
    { }

    public IpExceptionBase(IPAddress? ip)
        => IP = ip;

    public IpExceptionBase(IPAddress? ip, string? message)
        : base(message)
        => IP = ip;

    // Extra Data

    public IPAddress? IP { get; } = null;
}