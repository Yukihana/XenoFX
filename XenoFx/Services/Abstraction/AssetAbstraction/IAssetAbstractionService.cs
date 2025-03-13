using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Abstraction.AssetAbstraction;

public interface IAssetAbstractionService
{
    string[] GetHave(string searchString, CancellationToken ctoken = default);
}