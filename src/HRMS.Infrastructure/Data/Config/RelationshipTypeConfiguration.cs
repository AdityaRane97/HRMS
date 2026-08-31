using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

public class RelationshipTypeConfiguration : IEntityTypeConfiguration<RelationshipType>
{
    public void Configure(EntityTypeBuilder<RelationshipType> builder)
    {
        // Table configuration
        builder.ToTable("RelationshipTypes");

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

        // Indexes
        builder.HasIndex(x => x.Code).IsUnique()
            .HasName("ix_relationship_types_code_unique");

        builder.HasIndex(x => x.IsActive)
            .HasName("ix_relationship_types_active");

        // Soft delete support from BaseEntity
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
    }
}
