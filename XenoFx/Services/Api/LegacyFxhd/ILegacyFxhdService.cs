using System.Threading;
using System.Threading.Tasks;

namespace XenoFx.Services.Api.LegacyFxhd;

public interface ILegacyFxhdService
{
    Task<string[]> GetHaveAsync(CancellationToken ctoken = default);
}