using System;

namespace CSX.DotNet.Modules.AuthIpPin.Services.AuthApi.Models;

public class IpPinAuthContext
{
    public IpPinAuthContext(IpPinAuthDto dto)
        => Dto = dto ?? throw new ArgumentNullException(nameof(dto));

    // Data exposed to wrapping service (e.g. controller)

    public IpPinAuthDto Dto { get; }

    // Internal use only

    public SessionStates SessionState { get; set; } = SessionStates.Available;

    public AuthTokenStates AuthTokenState { get; set; } = AuthTokenStates.None;

    public bool IsRequestAuthorized
        => SessionState == SessionStates.Active
        && AuthTokenState == AuthTokenStates.Matched;
}