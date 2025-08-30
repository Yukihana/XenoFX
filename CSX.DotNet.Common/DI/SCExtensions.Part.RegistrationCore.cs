using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace CSX.DotNet.Common.DI;

public static partial class OrchestratorExtensions
{
    public static IServiceCollection AddMarkedServices<TMarker>(
        this IServiceCollection services,
        params Assembly[]? assemblies)
    {
        // Default to entry assembly if none provided
        assemblies ??= [];
        if (assemblies.Length == 0 &&
            Assembly.GetEntryAssembly() is Assembly assembly)
        {
            assemblies = [assembly];
        }

        // Find all classes that implement the marker interface
        var types = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(t => t.IsClass && !t.IsAbstract && typeof(TMarker).IsAssignableFrom(t));

        foreach (var impl in types)
        {
            // Determine lifetime via attribute, default to transient
            var lifetimeAttr = impl.GetCustomAttribute<DependencyLifetimeAttribute>();
            var lifetime = lifetimeAttr?.Lifetime ?? ServiceLifetime.Transient;

            // Register all interfaces except the marker
            var serviceInterfaces = impl.GetInterfaces().Where(i => i != typeof(TMarker));
            foreach (var iface in serviceInterfaces)
            {
                var descriptor = new ServiceDescriptor(iface, impl, lifetime);
                services.Add(descriptor);
            }
        }

        return services;
    }
}