using CSX.DotNet.Modules.FileUploader.Environment.Configuration;
using CSX.DotNet.Modules.FileUploader.Services.Configuration;
using CSX.DotNet.Modules.FileUploader.Services.UploadApi;
using CSX.DotNet.Modules.FileUploader.Storage.Cleanup;
using CSX.DotNet.Modules.FileUploader.Storage.Sidecar;
using CSX.DotNet.Modules.FileUploader.Storage.UploadCache;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace CSX.DotNet.Modules.FileUploader.Environment;

public static partial class Factory
{
    [Obsolete("This method is not implemented.", true)]
    public static IServiceCollection AddFileUploaderDatabases(
        this IServiceCollection services,
        FileUploaderConfiguration config,
        CancellationToken ctoken = default)
    {
        throw new NotImplementedException();

        // ctoken.ThrowIfCancellationRequested();
        // return services;
    }

    public static IServiceCollection AddFileUploaderServices(
        this IServiceCollection services,
        FileUploaderConfiguration config,
        CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Configuration
        services.AddConfigurationService(config);

        // Storage
        services.AddSingleton<IUploadCacheService, UploadCacheService>();
        services.AddSingleton<ISidecarService, SidecarService>();
        services.AddSingleton<ICleanupService, CleanupService>();

        // Api
        services.AddSingleton<IUploadApiService, UploadApiService>();

        // Middleware

        return services;
    }
}