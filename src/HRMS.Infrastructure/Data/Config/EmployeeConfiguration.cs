using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

/// <summary>
/// Entity Framework Core configuration for Employee entity.
/// </summary>
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Table
        builder.ToTable("Employees", "dbo");

        // Keys
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasDefaultValueSql("NEWID()");

        // Concurrency
        builder.Property(e => e.RowVersion).IsRowVersion();

        // Properties
        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(e => e.Email).IsUnique();

        builder.Property(e => e.EmployeeCode).HasMaxLength(50).IsRequired();
        builder.HasIndex(e => e.EmployeeCode).IsUnique();

        builder.Property(e => e.PhoneNumber).HasMaxLength(20);
        builder.Property(e => e.EmployeeDepartment).HasMaxLength(100);
        builder.Property(e => e.Designation).HasMaxLength(100);
        builder.Property(e => e.JoinDate).IsRequired();
        builder.Property(e => e.ResignationDate);
        builder.Property(e => e.EmploymentStatus).HasMaxLength(50).HasDefaultValue("Active");
        builder.Property(e => e.EmploymentType).HasMaxLength(50).HasDefaultValue("FullTime");

        builder.Property(e => e.ManagerId);
        builder.Property(e => e.OrganizationId).IsRequired();

        builder.Property(e => e.IdentityProvider).HasMaxLength(50);
        builder.Property(e => e.ExternalUserId).HasMaxLength(256);
        builder.Property(e => e.IsActive).HasDefaultValue(true);

        builder.Property(e => e.ProfilePhotoUrl).HasMaxLength(512);
        builder.Property(e => e.Address).HasMaxLength(255);
        builder.Property(e => e.City).HasMaxLength(100);
        builder.Property(e => e.State).HasMaxLength(100);
        builder.Property(e => e.Country).HasMaxLength(100);
        builder.Property(e => e.PostalCode).HasMaxLength(20);

        // Audit properties
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.UpdatedAt);
        builder.Property(e => e.UpdatedBy).HasMaxLength(256);
        builder.Property(e => e.IsDeleted).HasDefaultValue(false);
        builder.Property(e => e.DeletedAt);
        builder.Property(e => e.DeletedBy).HasMaxLength(256);

        // Relationships
        builder.HasOne(e => e.Manager)
            .WithMany(e => e.DirectReports)
            .HasForeignKey(e => e.ManagerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne(e => e.Organization)
            .WithMany(o => o.Employees)
            .HasForeignKey(e => e.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Indexes
        builder.HasIndex(e => e.OrganizationId);
        builder.HasIndex(e => e.ManagerId);
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.EmploymentStatus);
        builder.HasIndex(e => e.IsDeleted);
        builder.HasIndex(e => new { e.OrganizationId, e.IsDeleted, e.EmploymentStatus });
    }
}
