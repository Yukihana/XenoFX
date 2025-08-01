using System.Net;

namespace CSX.DotNet.Common.Data.Exceptions;

public class MalformedIpException : IpExceptionBase
{
    public MalformedIpException() : base()
    { }

    public MalformedIpException(string? message) : base(message)
    { }

    public MalformedIpException(IPAddress? ip) : base(ip)
    { }

    public MalformedIpException(IPAddress? ip, string? message) : base(ip, message)
    { }
}