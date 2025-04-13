using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;
using XenoFx.Environment;

namespace XenoServe;

public class Program
{
    public async static Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

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

    public async static Task<WebApplicationBuilder> GetBuilderAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Attach Logger
        builder.Host.UseSerilog();

        // Add services to the container.
        await builder.Services.AddXenoFxAsync(builder.Configuration);

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

        return builder;
    }

    private async static Task<WebApplication> PreInitializeAsync(WebApplication app, CancellationToken ctoken = default)
    {
        ctoken.ThrowIfCancellationRequested();

        // Initialize service groups
        await app.Services.PreInitializeXenoFxAsync(ctoken);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}