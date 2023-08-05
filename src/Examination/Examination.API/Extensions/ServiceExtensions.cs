using MongoDB.Driver;

namespace Examination.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMongoClient>(c =>
            {
                var user = configuration.GetValue<string>("DatabaseSettings:User");
                var password = configuration.GetValue<string>("DatabaseSettings:Password");
                var server = configuration.GetValue<string>("DatabaseSettings:Server");
                var databaseName = configuration.GetValue<string>("DatabaseSettings:DatabaseName");
                return new MongoClient(
                    "mongodb://" + user + ":" + password + "@" + server + "/" + databaseName + "?authSource=admin");
            });
            return services;
        }
    }
}
