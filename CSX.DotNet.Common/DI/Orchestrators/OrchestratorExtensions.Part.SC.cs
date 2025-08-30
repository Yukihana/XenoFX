using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CSX.DotNet.Common.DI.Orchestrators;

public static partial class OrchestratorExtensions
{
    public static IServiceCollection AddOrchestrators(
        this IServiceCollection services,
        params Assembly[]? assemblies)
    {
        return services.AddMarkedServices<IOrchestrator>(assemblies);
    }
}