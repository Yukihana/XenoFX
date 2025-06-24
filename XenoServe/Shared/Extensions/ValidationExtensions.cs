using CSX.Common.Data.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace XenoServe.Shared.Extensions;

public static class ValidationExtensions
{
    public static void ThrowIfNull([NotNull] this IPAddress? ip)
    {
        if (ip is null)
            throw new MalformedIpException("IP address could not be determined.");
    }
}