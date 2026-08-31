using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        // Table configuration
        builder.ToTable("LeaveTypes");

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

        builder.Property(x => x.DefaultAllocation)
            .HasPrecision(8, 2);

        builder.Property(x => x.Unit)
            .HasMaxLength(20)
            .HasDefaultValue("Days");

        builder.Property(x => x.MaxCarryForwardAmount)
            .HasPrecision(8, 2);

        // Indexes
        builder.HasIndex(x => x.Code).IsUnique()
            .HasName("ix_leave_types_code_unique");

        builder.HasIndex(x => x.IsActive)
            .HasName("ix_leave_types_active");

        // Soft delete support from BaseEntity
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
    }
}
