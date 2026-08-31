using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        // Table configuration
        builder.ToTable("LeaveBalances");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AvailableBalance)
            .HasPrecision(8, 2);

        builder.Property(x => x.UsedBalance)
            .HasPrecision(8, 2);

        builder.Property(x => x.AccruedBalance)
            .HasPrecision(8, 2);

        builder.Property(x => x.CarryForwardBalance)
            .HasPrecision(8, 2);

        builder.Property(x => x.FinancialYear)
            .IsRequired()
            .HasMaxLength(9); // e.g. "2024-25"

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.LeaveType)
            .WithMany(x => x.LeaveBalances)
            .HasForeignKey(x => x.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(x => x.EmployeeId)
            .HasName("ix_leave_balances_employee_id");

        builder.HasIndex(x => x.LeaveTypeId)
            .HasName("ix_leave_balances_leave_type_id");

        builder.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId })
            .HasName("ix_leave_balances_employee_leavetype").IsUnique();

        builder.HasIndex(x => new { x.EmployeeId, x.FinancialYear })
            .HasName("ix_leave_balances_employee_year");

        // Soft delete support from BaseEntity
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
    }
}
