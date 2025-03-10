using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Api.AssetSearch;

public interface IAssetSearchService
{
    Task<string[]> GetHave(CancellationToken ctoken = default);

    Task<string[]> GetHave(string searchString, CancellationToken ctoken = default);

    Task<Dictionary<string, float>> Search(string searchString, CancellationToken ctoken = default);
}