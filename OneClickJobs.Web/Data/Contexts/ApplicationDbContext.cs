using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using OneClickJobs.Domain.Entities;

namespace OneClickJobs.Web.Data.Contexts;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<Application> Applications { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var relationship in entityType.GetForeignKeys())
            {
                relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;
            }

            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType.IsEnum)
                {
                    var enumType = property.ClrType;
                    var converterType = typeof(EnumToStringConverter<>).MakeGenericType(enumType);
                    var converter = Activator.CreateInstance(converterType);

                    property.SetValueConverter((ValueConverter)converter!);
                }
            }
        }

        base.OnModelCreating(builder);
    }
}
