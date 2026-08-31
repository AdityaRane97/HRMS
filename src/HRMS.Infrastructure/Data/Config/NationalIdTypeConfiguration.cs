using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

public class NationalIdTypeConfiguration : IEntityTypeConfiguration<NationalIdType>
{
    public void Configure(EntityTypeBuilder<NationalIdType> builder)
    {
        // Table configuration
        builder.ToTable("NationalIdTypes");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ValidationPattern)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(x => x.Code).IsUnique()
            .HasName("ix_national_id_types_code_unique");

        builder.HasIndex(x => new { x.Country, x.Code })
            .HasName("ix_national_id_types_country_code");

        builder.HasIndex(x => x.IsActive)
            .HasName("ix_national_id_types_active");

        // Soft delete support from BaseEntity
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
    }
}
