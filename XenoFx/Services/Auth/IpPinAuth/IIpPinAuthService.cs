using CSX.Common.Data.Placeholders;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Auth.IpPinAuth.Models;

namespace XenoFx.Services.Auth.IpPinAuth;

public interface IIpPinAuthService
{
    // Middleware

    Task<bool> AuthorizeIpAsync(
        IPAddress ip,
        CancellationToken ctoken = default);

    // Payload

    Task GetStatusAsync(
        IpPinAuthDto<Unit, IpPinAuthClientInfo> dto,
        CancellationToken ctoken = default);

    Task GeneratePinAsync(
        IpPinAuthDto<Unit, int> dto,
        CancellationToken ctoken = default);

    // Login / Logout

    Task LoginAsync(
        IpPinAuthDto<int?, IpPinAuthentication> dto,
        CancellationToken ctoken = default);

    Task LogoutAsync(
        IpPinAuthDto<Unit, Unit> dto,
        CancellationToken ctoken = default);

    // Ip

    Task ActivateIpAsync(
        IpPinAuthDto<TimeSpan?, Unit> dto,
        CancellationToken ctoken = default);

    Task DeactivateIpAsync(
        IpPinAuthDto<Unit, Unit> dto,
        CancellationToken ctoken = default);
}