using System;

namespace XenoFx.Services.Auth.IpPinAuth.Models;

public class IpPinAuthentication
{
    public IpPinAuthentication(
        string authToken,
        DateTimeOffset expiry)
    {
        AuthToken = authToken;
        Expiry = expiry;
    }

    public string AuthToken { get; }
    public DateTimeOffset Expiry { get; }
}