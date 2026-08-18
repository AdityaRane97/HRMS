using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

/// <summary>
/// EF Core configuration for PayrollRecord entity.
/// Defines table structure, relationships, constraints, and indexes.
/// TODO: User to review and adjust precision/scale for salary fields if needed
/// </summary>
public class PayrollRecordConfiguration : IEntityTypeConfiguration<PayrollRecord>
{
    public void Configure(EntityTypeBuilder<PayrollRecord> builder)
    {
        builder.ToTable("PayrollRecords");

        // Primary Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.EmployeeId).IsRequired();
        builder.Property(p => p.PayrollMonth).IsRequired();
        builder.Property(p => p.PaymentDate).IsRequired();

        // Salary Components
        builder.Property(p => p.BaseSalary)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.HouseRentAllowance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.MedicalAllowance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.TransportAllowance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.OtherAllowances)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        // Deductions
        builder.Property(p => p.IncomeTax)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.ProvidentFund)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.HealthInsurance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.OtherDeductions)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        // Calculated Fields
        builder.Property(p => p.GrossSalary)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.TotalDeductions)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.NetSalary)
            .HasPrecision(18, 2)
            .IsRequired();

        // Status
        builder.Property(p => p.PaymentStatus)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Remarks)
            .HasMaxLength(500);

        builder.Property(p => p.ReferenceNumber)
            .HasMaxLength(100);

        // Relationships
        builder.HasOne(p => p.Employee)
            .WithMany() // TODO: Consider adding ICollection<PayrollRecord> to Employee if needed
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(p => p.EmployeeId);
        builder.HasIndex(p => new { p.EmployeeId, p.PayrollMonth }).IsUnique().HasDatabaseName("IX_PayrollRecords_EmployeeIdMonth");

        // Audit Fields (inherited from BaseEntity)
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt);
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
    }
}
