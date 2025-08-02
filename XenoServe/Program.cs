using CSX.DotNet.Modules.AuthIpPin.Environment;
using CSX.DotNet.Modules.FileUploader.Environment;

// using CSX.DotNet.Modules.XenoFx.Environment; // TODO
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment;
using XenoServe.Environment;
using XenoServe.Environment.Modules;

namespace XenoServe;

public class Program
{
    public async static Task Main(string[] args)
    {
        // Create a serilog bootstrap logger
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            // Build
            var builder = await GetBuilderAsync(args);
            WebApplication app = builder.Build();

            // Preinitialize
            await PreInitializeAsync(app, CancellationToken.None);

            // Run
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    public async static Task<WebApplicationBuilder> GetBuilderAsync(
        string[] args,
        CancellationToken ctoken = default)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Attach Logger (Apply actual serilog configuration from appsettings.json)
        builder.Host.UseSerilog((context, services, config) => config
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());

        // Read runtime profile
        var xsConfig = await builder.Configuration
            .GetXenoServeConfigAsync(args, ctoken);

        // Add services to the container.
        await builder.Services.AddXenoFxAsync(XenoFxOptions.CreateFrom(xsConfig), ctoken);
        await builder.Services.AddFileUploaderAsync(FileUploaderOptions.CreateFrom(xsConfig), ctoken);
        await builder.Services.AddAuthIpPinAsync(AuthIpPinOptions.CreateFrom(xsConfig), ctoken);

        // Add orchestrators
        builder.Services.AddOrchestrators();

        // builder.Services.AddControllers(); // Api Only
        builder.Services.AddControllersWithViews(); // Also handles views

        // Replace default view locations with feature-based locations
        builder.Services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationFormats.Clear();                                // Clear default locations
            options.ViewLocationFormats.Add("/Features/{1}/Views/{0}.cshtml");  // Controller-based views
            options.ViewLocationFormats.Add("/Features/{1}/{0}.cshtml");
            options.ViewLocationFormats.Add("/Shared/Views/{0}.cshtml");        // Shared views
        });

        // Swagger / Open API
        builder.Services.AddOpenApi();  // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // If debugging for non-loopback addresses, uncomment line below
        if (builder.Environment.IsDevelopment()) { builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(9600)); }

        return builder;
    }

    private async static Task<WebApplication> PreInitializeAsync(WebApplication app, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Redirect the default path to where it's needed
        app.MapGet("/", () => Results.Redirect("/legacy/browse"));

        // Enable IpPinAuthMiddleware
        app.AddXenoFxMiddlewares();
        app.AddAuthIpPinMiddlewares();

        // Enable static files
        app.MapStaticAssets();
        app.UseDefaultFiles(new DefaultFilesOptions()
        {
            DefaultFileNames = ["default.html", "index.html"], // Default file to serve
        });

        // Initialize service groups
        await app.Services.PreInitializeXenoFxAsync(ctoken);
        await app.Services.PreInitializeAuthIpPinAsync(ctoken);

        // Swagger first, so it’s served directly by ASP.NET Core
        if (app.Environment.IsDevelopment())
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        // Map controllers before the SPA
        app.MapControllers();

        // Enable SPA development server proxy (Assumes Vite frontend is running)
        /*
        if (app.Environment.IsDevelopment())
        {
            app.UseSpa(spa =>
            {
                spa.UseProxyToSpaDevelopmentServer("https://localhost:5173"); // Matches Vite dev port
            });
        }*/

        // Enable fallback to SPA
        app.MapFallbackToFile("/index.html");

        return app;
    }
}