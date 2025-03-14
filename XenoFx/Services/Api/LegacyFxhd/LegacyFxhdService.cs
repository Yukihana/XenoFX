using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Services.Storage.AssetPresence;

namespace XenoFx.Services.Api.LegacyFxhd;

public sealed partial class LegacyFxhdService : ILegacyFxhdService
{
    private readonly IAssetPresenceService _assetPresence;

    public LegacyFxhdService(IAssetPresenceService assetPresence)
    {
        _assetPresence = assetPresence;
    }

    public Task<string[]> GetHaveAsync(CancellationToken ctoken = default)
    {
        throw new NotImplementedException();
    }
}