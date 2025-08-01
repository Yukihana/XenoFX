using CSX.DotNet.Common.Data.Placeholders;
using CSX.DotNet.Modules.AuthIpPin.Services.AuthApi.Models;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CSX.DotNet.Modules.AuthIpPin.Services.AuthApi;

public interface IAuthApiService
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