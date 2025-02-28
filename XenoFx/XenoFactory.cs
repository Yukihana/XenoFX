using Microsoft.Extensions.DependencyInjection;
using System;

namespace XenoFx;

public static partial class XenoFactory
{
    // TODO Documentation: Registers the framework's services with the provided IServiceCollection.
    public static IServiceCollection AddXenoFx(this IServiceCollection services)
    {
        // Storage layer

        // Analysis layer

        // Abstraction layer

        // Processing layer

        // API layer

        return services;
    }

    // TODO Documentation:  Handles pre-initialization for the framework before consumption.
    public static IServiceProvider PreInitializeXenoFx(this IServiceProvider provider)
    {
        // Start database connections

        // Warm up the file tracker

        // Detect assets and run quick-mode integrity tests

        // Register available assets for consumption

        return provider;
    }

    // TODO Documentation: Activates parallel subroutines
    public static IServiceProvider Activate(this IServiceProvider provider)
    {
        // Start database connections

        // Warm up the file tracker

        // Detect assets and run quick-mode integrity tests

        // Register available assets for consumption

        return provider;
    }

    // TODO Documentation: Creates a default container for the framework's services.
    public static IServiceProvider Create()
    {
        ServiceCollection services = new();
        services.AddXenoFx();

        ServiceProvider sp = services.BuildServiceProvider();
        return sp;
    }

    public static IServiceProvider CreateAndRun()
        => Create()
        .PreInitializeXenoFx()
        .Activate();
}