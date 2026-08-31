using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

public class FamilyContactConfiguration : IEntityTypeConfiguration<FamilyContact>
{
    public void Configure(EntityTypeBuilder<FamilyContact> builder)
    {
        // Table configuration
        builder.ToTable("FamilyContacts");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ContactName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Relationship)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(255);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(x => x.Employee)
            .WithMany(x => x.FamilyContacts)
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(x => x.EmployeeId)
            .HasName("ix_family_contacts_employee_id");

        builder.HasIndex(x => new { x.EmployeeId, x.IsEmergencyContact })
            .HasName("ix_family_contacts_employee_emergency");

        builder.HasIndex(x => new { x.EmployeeId, x.IsPrimaryContact })
            .HasName("ix_family_contacts_employee_primary");

        // Soft delete support from BaseEntity
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
    }
}
