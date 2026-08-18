using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

/// <summary>
/// EF Core configuration for AttendanceLog entity.
/// Defines table structure, relationships, constraints, and indexes.
/// TODO: User to adjust indexes based on query patterns (by date, by employee, etc.)
/// </summary>
public class AttendanceLogConfiguration : IEntityTypeConfiguration<AttendanceLog>
{
    public void Configure(EntityTypeBuilder<AttendanceLog> builder)
    {
        builder.ToTable("AttendanceLogs");

        // Primary Key
        builder.HasKey(a => a.Id);

        // Properties
        builder.Property(a => a.EmployeeId).IsRequired();
        builder.Property(a => a.AttendanceDate).IsRequired();

        // Check-in/out times
        builder.Property(a => a.CheckInTime);
        builder.Property(a => a.CheckOutTime);

        // Computed Fields
        builder.Property(a => a.WorkedHours)
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        builder.Property(a => a.AttendanceStatus)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Present");

        // Additional Info
        builder.Property(a => a.Location)
            .HasMaxLength(100);

        builder.Property(a => a.Remarks)
            .HasMaxLength(500);

        // Manager/HR Override
        builder.Property(a => a.ApprovedBy);
        builder.Property(a => a.ApprovedAt);

        builder.Property(a => a.ApprovalRemarks)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(a => a.Employee)
            .WithMany() // TODO: Consider adding ICollection<AttendanceLog> to Employee if needed
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(a => a.EmployeeId);
        builder.HasIndex(a => new { a.EmployeeId, a.AttendanceDate }).IsUnique().HasDatabaseName("IX_AttendanceLogs_EmployeeIdDate");
        builder.HasIndex(a => a.AttendanceDate); // For date-range queries

        // Audit Fields (inherited from BaseEntity)
        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.UpdatedAt);
        builder.Property(a => a.IsDeleted).HasDefaultValue(false);
    }
}
