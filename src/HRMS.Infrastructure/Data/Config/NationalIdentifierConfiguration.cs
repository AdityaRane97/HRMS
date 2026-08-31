using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

public class NationalIdentifierConfiguration : IEntityTypeConfiguration<NationalIdentifier>
{
    public void Configure(EntityTypeBuilder<NationalIdentifier> builder)
    {
        // Table configuration
        builder.ToTable("NationalIdentifiers");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IdType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IdNumber)
            .IsRequired()
            .HasMaxLength(500); // Extended to support long ID numbers

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(x => x.Employee)
            .WithMany(x => x.NationalIdentifiers)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(x => x.EmployeeId)
            .HasName("ix_national_identifiers_employee_id");

        builder.HasIndex(x => new { x.EmployeeId, x.IsPrimary })
            .HasName("ix_national_identifiers_employee_primary");

        // Soft delete support from BaseEntity
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
    }
}
