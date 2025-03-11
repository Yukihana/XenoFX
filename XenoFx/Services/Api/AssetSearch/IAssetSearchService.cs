using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Api.AssetSearch;

public interface IAssetSearchService
{
    Task<string[]> GetHaveAsync(CancellationToken ctoken = default);

    Task<string[]> GetHaveAsync(string searchString, CancellationToken ctoken = default);

    Task<Dictionary<string, float>> SearchAsync(string searchString, CancellationToken ctoken = default);
}