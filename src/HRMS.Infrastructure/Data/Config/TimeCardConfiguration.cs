using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

public class TimeCardConfiguration : IEntityTypeConfiguration<TimeCard>
{
    public void Configure(EntityTypeBuilder<TimeCard> builder)
    {
        // Table configuration
        builder.ToTable("TimeCards");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PeriodStart)
            .IsRequired();

        builder.Property(x => x.PeriodEnd)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Draft");

        builder.Property(x => x.TotalHours)
            .HasPrecision(8, 2); // Max 999999.99 hours

        builder.Property(x => x.ManagerRemarks)
            .HasMaxLength(500);

        builder.Property(x => x.HRRemarks)
            .HasMaxLength(500);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(x => x.Employee)
            .WithMany()
            .HasForeignKey(x => x.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(x => x.EmployeeId)
            .HasName("ix_time_cards_employee_id");

        builder.HasIndex(x => new { x.EmployeeId, x.PeriodStart })
            .HasName("ix_time_cards_employee_period");

        builder.HasIndex(x => x.Status)
            .HasName("ix_time_cards_status");

        builder.HasIndex(x => new { x.EmployeeId, x.Status })
            .HasName("ix_time_cards_employee_status");

        // Soft delete support from BaseEntity
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
    }
}
