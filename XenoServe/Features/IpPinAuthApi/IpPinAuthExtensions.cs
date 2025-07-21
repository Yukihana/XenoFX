using CSX.DotNet.Modules.AuthIpPin.Services.AuthApi.Models;
using XenoServe.Features.IpPinAuthApi.Payloads;

namespace XenoServe.Features.IpPinAuthApi;

public static class IpPinAuthExtensions
{
    public static ClientInfoResponse MapToResponse(
        this IpPinAuthClientInfo clientInfo)
    {
        return new ClientInfoResponse
        {
            CurrentIp = clientInfo.CurrentIP,
            IsAuthorized = clientInfo.IsAuthorized,
            // If authorized
            TokenExpiresAt = clientInfo.TokenExpiresAt,
            ActiveIp = clientInfo.ActiveIP,
            // If active IP is set
            LeaseStartTime = clientInfo.LeaseStartTime,
            LeaseEndTime = clientInfo.LeaseEndTime
        };
    }
}