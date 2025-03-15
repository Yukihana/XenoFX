using System;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Api.AssetApi;

[Obsolete("Superceded by partitioned api services.")]
public sealed partial class AssetApiService : IAssetApiService
{
    public async Task<string[]> QueryHaveFileAsync(string file, CancellationToken ctoken = default)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }

    public async Task<byte[]> GetThumbnailAsync(string file, CancellationToken ctoken = default)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }

    public async Task<byte[]> GetThumbnailAsync(byte[] hash, CancellationToken ctoken = default)
    {
        await Task.Yield();
        throw new NotImplementedException();
    }
}