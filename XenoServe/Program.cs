using Microsoft.AspNetCore.Mvc.Razor;
using Serilog;
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
            // Build the application
            WebApplication app = await BuildAsync(args);

            // Run it here, not inside the method that built it.
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

    public async static Task<WebApplication> BuildAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Attach Logger
        builder.Host.UseSerilog();

        // Add services to the container.
        await builder.Services.AddXenoFxAsync(builder.Configuration);

        // builder.Services.AddControllers(); // Api Only
        builder.Services.AddControllersWithViews(); // Needed for pages

        // Replace default view locations with feature-based locations
        builder.Services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationFormats.Clear();                                // Clear default locations
            options.ViewLocationFormats.Add("/Features/{1}/Views/{0}.cshtml");  // Controller-based views
            options.ViewLocationFormats.Add("/Features/{1}/{0}.cshtml");
            options.ViewLocationFormats.Add("/Shared/Views/{0}.cshtml");        // Shared views
        });

        builder.Services.AddOpenApi();  // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Build DI
        var app = builder.Build();

        // Boot up storage services
        app.Services.PreInitializeXenoFx();

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