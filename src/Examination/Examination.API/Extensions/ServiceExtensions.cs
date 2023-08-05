using Examination.Application.Commands.V1.StartExam;
using Examination.Application.Mapping;
using Examination.Domain.AggregateModels.ExamAggregate;
using Examination.Domain.AggregateModels.ExamResultAggregate;
using Examination.Domain.AggregateModels.UserAggregate;
using Examination.Infrastructure.MongoDb.Repositories;
using Examination.Infrastructure.MongoDb.SeedWork;
using MediatR;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

namespace Examination.API.Extensions
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(
                          options =>
                          {
                              // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                              // note: the specified format code will format the version as "'v'major[.minor][-status]"
                              options.GroupNameFormat = "'v'VVV";

                              // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                              // can also be used to control the format of the API version in route templates
                              options.SubstituteApiVersionInUrl = true;
                          });

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

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(c => c.GetService<IMongoClient>()?.StartSession());
            services.AddAutoMapper(cfg => { cfg.AddProfile(new MappingProfile()); });
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(StartExamCommandHandler).Assembly));

            services.AddControllers();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Examination.API V1", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Examination.API V2", Version = "v2" });
            });
            services.Configure<ExamSettings>(configuration);

            services.AddTransient<IExamRepository, ExamRepository>();
            services.AddTransient<IExamResultRepository, ExamResultRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            return services;
        }
    }
}
