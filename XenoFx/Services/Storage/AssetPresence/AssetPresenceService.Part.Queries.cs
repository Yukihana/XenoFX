using System;
using System.Linq;

namespace XenoFx.Services.Storage.AssetPresence;

public sealed partial class AssetPresenceService
{
    // FxHD

    public string[] GetPaths(Func<string, bool> validationCallback)
        => [.. Presences.Keys.Where(validationCallback)];
}