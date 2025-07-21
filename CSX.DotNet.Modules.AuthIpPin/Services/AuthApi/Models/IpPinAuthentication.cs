using System;

namespace CSX.DotNet.Modules.AuthIpPin.Services.AuthApi.Models;

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