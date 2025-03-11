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

        // Add services to the container.
        await builder.Services.AddXenoFxAsync(builder.Configuration);

        /*
        builder.Services.AddSingleton(new NitefoxTracker());
        builder.Services.AddScoped<NitefoxCore>();
        */

        builder.Services.AddControllers();

        builder.Services.AddOpenApi();  // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Build DI
        var app = builder.Build();

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