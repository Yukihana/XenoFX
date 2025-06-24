using System;

namespace XenoFx.Services.Auth.IpPinAuth.Models;

public class IpPinAuthContext
{
    // Data exposed to wrapping service (e.g. controller)

    public IpPinAuthDto Dto { get; set; } = new();

    // Internal use only

    internal SessionStates SessionState { get; set; } = SessionStates.Available;

    internal AuthTokenStates AuthTokenState { get; set; } = AuthTokenStates.None;

    // LifeCycle

    public IpPinAuthContext() : this(new IpPinAuthDto())
    { }

    public IpPinAuthContext(IpPinAuthDto dto)
    {
        Dto = dto
            ?? throw new ArgumentNullException(nameof(dto));
    }
}