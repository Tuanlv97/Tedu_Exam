using Identity.Admin.Api;
using Identity.Admin.Api.Databases;
using Identity.Admin.Api.Models;
using Identity.Admin.Api.Services;
using Identity.Admin.API.Database;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Log.Information($"Start {builder.Environment.ApplicationName} up");
try
{

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
           sqlServerOptionsAction: sqlOptions =>
           {
               sqlOptions.MigrationsAssembly(migrationsAssembly);
               sqlOptions.EnableRetryOnFailure(
                   maxRetryCount: 5,
                   maxRetryDelay: TimeSpan.FromSeconds(30),
                   errorNumbersToAdd: null);
           })

       );

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.Configure<AppSettings>(builder.Configuration);


    builder.Services.AddIdentityServer(c =>
    {
        c.IssuerUri = "Https://tedu.com.vn";
        c.Authentication.CookieLifetime = TimeSpan.FromHours(1);

    })
         .AddDeveloperSigningCredential()
         .AddAspNetIdentity<ApplicationUser>()
         .AddConfigurationStore(options =>
         {
             options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
             {
                 sqlOptions.MigrationsAssembly(migrationsAssembly);
                 sqlOptions.EnableRetryOnFailure(
                     maxRetryCount: 5,
                     maxRetryDelay: TimeSpan.FromSeconds(30),
                     errorNumbersToAdd: null);
             });
         })
         .AddOperationalStore(options =>
         {
             options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString,
         sqlServerOptionsAction: sqlOptions =>
         {
             sqlOptions.MigrationsAssembly(migrationsAssembly);
             sqlOptions.EnableRetryOnFailure(
                  maxRetryCount: 5,
                  maxRetryDelay: TimeSpan.FromSeconds(30),
                  errorNumbersToAdd: null);

         });
         });

    builder.Services.AddTransient<IProfileService, ProfileService>();

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


    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity.API", Version = "v1" });
    });


    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity.API v1"));
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();

    app.UseRouting();

    app.UseIdentityServer();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        try
        {
            var db = services.GetRequiredService<PersistedGrantDbContext>();
            db.Database.Migrate();

            var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
            applicationDbContext.Database.Migrate();

            var configurationDbContext = services.GetRequiredService<ConfigurationDbContext>();
            configurationDbContext.Database.Migrate();

            var env = services.GetService<IWebHostEnvironment>();
            var logger = services.GetService<ILogger<ApplicationDbContextSeed>>();
            var settings = services.GetService<IOptions<AppSettings>>();
            new ApplicationDbContextSeed()
                           .SeedAsync(applicationDbContext, env, logger, settings)
                           .Wait();

            new ConfigurationDbContextSeed()
                             .SeedAsync(configurationDbContext, builder.Configuration)
                             .Wait();

        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
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