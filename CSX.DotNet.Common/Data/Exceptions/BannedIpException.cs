using System.Net;

namespace CSX.DotNet.Common.Data.Exceptions;

public class BannedIpException : IpExceptionBase
{
    public BannedIpException() : base()
    { }

    public BannedIpException(string? message) : base(message)
    { }

    public BannedIpException(IPAddress? ip) : base(ip)
    { }

    public BannedIpException(IPAddress? ip, string? message) : base(ip, message)
    { }
}