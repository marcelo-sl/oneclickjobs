using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneClickJobs.Web.Data.Entities;

namespace OneClickJobs.Web.Data.Mappings;

public sealed class ResumeMapping : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.ToTable("Resumes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.Property(x => x.FileName)
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(x => x.FileContent).IsRequired();
    }
}
