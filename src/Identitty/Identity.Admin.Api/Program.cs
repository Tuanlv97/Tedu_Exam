using Identity.Admin.Api;
using Identity.Admin.Api.Databases;
using Identity.Admin.Api.Models;
using Identity.Admin.Api.Services;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
string appName = typeof(Program).Namespace;
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


#region Add Identity User

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


#endregion

Serilog.ILogger CreateSerilogLogger(IConfiguration configuration)
{
    return new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .Enrich.WithProperty("ApplicationContext", appName)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();
}





builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity.API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();

app.Run();
