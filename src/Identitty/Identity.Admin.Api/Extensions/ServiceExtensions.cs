//namespace Identity.Admin.Api.Extensions
//{
//    public static class ServiceExtensions
//    {
//        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
//           IConfiguration configuration)
//                {
//        //            var jwtSettings = configuration.GetSection(nameof(JwtSettings))
//        //                .Get<JwtSettings>();
//        //            services.AddSingleton(jwtSettings);

//        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings))
//            .Get<DatabaseSettings>();
//        services.AddSingleton(databaseSettings);

//        //            var apiConfiguration = configuration.GetSection(nameof(ApiConfiguration))
//        //                .Get<ApiConfiguration>();
//        //            services.AddSingleton(apiConfiguration);

//                    return services;
//                }

//    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
//    {
//        services.AddControllers();
//        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
//        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//        services.AddEndpointsApiExplorer();
//        services.ConfigureProductDbContext(configuration);
//        ////services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
//        return services;
//    }

//    private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services, IConfiguration configuration)
//    {
//        var databaseSettings = configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();
//        if (databaseSettings == null || string.IsNullOrEmpty(databaseSettings.ConnectionString))
//            throw new ArgumentNullException("Connection string is not configured.");

//        var builder = new MySqlConnectionStringBuilder(databaseSettings.ConnectionString);
//        services.AddDbContext<ProductContext>(m => m.UseMySql(builder.ConnectionString,
//            ServerVersion.AutoDetect(builder.ConnectionString), e =>
//            {
//                e.MigrationsAssembly("Product.API");
//                e.SchemaBehavior(MySqlSchemaBehavior.Ignore);
//            }));

//        return services;
//    }
//}
//}
