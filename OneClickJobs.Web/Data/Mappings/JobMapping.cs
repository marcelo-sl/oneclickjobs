using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using OneClickJobs.Domain.Entities;

namespace OneClickJobs.Web.Data.Mappings;

public sealed class JobMapping : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("Jobs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.Title)
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnType("varchar(30)")
            .IsRequired();
    }
}
