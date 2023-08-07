using Examination.API.Extensions;
using Examination.Infrastructure;
using Examination.Infrastructure.MongoDb.SeedWork;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;
using System.Net.Mime;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);



Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.WithProperty("ApplicationContext", typeof(Program).Namespace)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day, shared: true)
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

Log.Information($"Start {builder.Environment.ApplicationName} up");
try
{
    // Add services to the container.

    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddInfrastructure(builder.Configuration);
    var app = builder.Build();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Examination.API v1");
            c.SwaggerEndpoint("/swagger/v2/swagger.json", "Examination.API v2");


        });
    }

    app.UseHttpsRedirection();

    app.UseRouting();
    app.UseCors("CorsPolicy");
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        endpoints.MapHealthChecksUI(options => options.UIPath = "/hc-ui");
        endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
        {
            Predicate = r => r.Name.Contains("self")
        });
        endpoints.MapHealthChecks("/hc-details",
                    new HealthCheckOptions
                    {
                        ResponseWriter = async (context, report) =>
                        {
                            var result = JsonSerializer.Serialize(
                                new
                                {
                                    status = report.Status.ToString(),
                                    monitors = report.Entries.Select(e => new { key = e.Key, value = Enum.GetName(typeof(HealthStatus), e.Value.Status) })
                                });
                            context.Response.ContentType = MediaTypeNames.Application.Json;
                            await context.Response.WriteAsync(result);
                        }
                    }
                );

        endpoints.MapControllers();
    });

    builder.WebHost.CaptureStartupErrors(false);

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ExamMongoDbSeeding>>();
        var settings = services.GetRequiredService<IOptions<ExamSettings>>();
        var mongoClient = services.GetRequiredService<IMongoClient>();
        new ExamMongoDbSeeding()
            .SeedAsync(mongoClient, settings, logger)
            .Wait();
    }

    app.Run();

}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shutdown {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}

