using Microsoft.Extensions.DependencyInjection;

namespace CSX.DotNet.Modules.FileIndexing.Shared.PathValidation;

internal static partial class PathValidationExtensions
{
    internal static IServiceCollection AddPathValidation(
        this IServiceCollection services)
    {
        services.AddScoped<IPathValidationService, PathValidationService>();

        return services;
    }
}