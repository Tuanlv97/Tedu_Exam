using Identity.Admin.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Admin.Api.Databases.Configuration
{
    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(250);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(250);
        }
    }
}
