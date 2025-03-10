using System;

namespace XenoFx.Services.AssetPresence;

public interface IAssetPresenceService
{
    // Registrations

    void Create(string path, UInt128 id = default);

    void Remove(string path);

    void Remove(UInt128 id);

    // Queries

    bool IsAsset(string path);

    bool IsAvailable(UInt128 id);

    bool IsFile(string path);

    UInt128 GetAssetId(string path);

    string? GetPath(UInt128 id);
}