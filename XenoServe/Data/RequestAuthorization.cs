using System;
using System.Net;

namespace XenoServe.Data;

public class RequestAuthorization
{
    public IPAddress? IPAddress { get; set; } = null;
    public DateTimeOffset TimeStamp { get; set; } = DateTimeOffset.MinValue;
}