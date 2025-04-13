using Microsoft.AspNetCore.Http;
using System;
using XenoServe.Data.Exceptions;
using XenoServe.Shared.Data;

namespace XenoServe.Shared.Extensions;

public static partial class HttpRequestExtensions
{
    public static void EnsureSameOriginForPartial(this HttpRequest request)
    {
        // Retrieve Referer and Origin headers
        string referer = request.Headers.Referer.ToString();
        string origin = request.Headers.Origin.ToString();

        // Strict validation: both headers must be present
        if (string.IsNullOrEmpty(referer) || string.IsNullOrEmpty(origin))
            throw new MissingRefererOrOriginException();

        // Convert headers to URI for comparison
        Uri refererUri = new(referer);
        Uri originUri = new(origin);

        // Ensure that Referer and Origin have the same host
        if (!string.Equals(refererUri.Host, originUri.Host, StringComparison.OrdinalIgnoreCase))
            throw new CORSViolationException();
    }
}