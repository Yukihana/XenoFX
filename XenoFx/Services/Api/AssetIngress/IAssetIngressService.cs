using System;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Api.AssetIngress;

public interface IAssetIngressService
{
    Task EnqueueUploadAsync(
        string uploadMetadataPath,
        Func<string, CancellationToken, Task>? cleanupCallback,
        CancellationToken ctoken = default);
}