using Identity.Admin.Api.Databases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace Identity.Admin.Api.Factories
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
               .AddJsonFile("appsettings.json")
               .AddEnvironmentVariables()
               .AddUserSecrets(Assembly.GetAssembly(typeof(Program)))
               .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"), sqlServerOptionsAction: o => o.MigrationsAssembly(typeof(Program).Assembly.FullName));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
