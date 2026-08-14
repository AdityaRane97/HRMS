using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

/// <summary>
/// Entity Framework Core configuration for Organization entity.
/// </summary>
public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        // Table
        builder.ToTable("Organizations", "dbo");

        // Keys
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasDefaultValueSql("NEWID()");

        // Concurrency
        builder.Property(o => o.RowVersion).IsRowVersion();

        // Properties
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.Description).HasMaxLength(1000);
        builder.Property(o => o.RegistrationNumber).HasMaxLength(100).IsRequired();
        builder.HasIndex(o => o.RegistrationNumber).IsUnique();

        builder.Property(o => o.Industry).HasMaxLength(100);
        builder.Property(o => o.Address).HasMaxLength(255);
        builder.Property(o => o.City).HasMaxLength(100);
        builder.Property(o => o.State).HasMaxLength(100);
        builder.Property(o => o.Country).HasMaxLength(100);
        builder.Property(o => o.PostalCode).HasMaxLength(20);
        builder.Property(o => o.PhoneNumber).HasMaxLength(20);
        builder.Property(o => o.Email).HasMaxLength(256);
        builder.Property(o => o.Website).HasMaxLength(256);

        // Audit properties
        builder.Property(o => o.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(o => o.CreatedBy).HasMaxLength(256);
        builder.Property(o => o.UpdatedAt);
        builder.Property(o => o.UpdatedBy).HasMaxLength(256);
        builder.Property(o => o.IsDeleted).HasDefaultValue(false);
        builder.Property(o => o.DeletedAt);
        builder.Property(o => o.DeletedBy).HasMaxLength(256);

        // Relationships
        builder.HasMany(o => o.Employees)
            .WithOne(e => e.Organization)
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(o => o.IsDeleted);
    }
}
