using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

/// <summary>
/// EF Core configuration for LeaveRequest entity.
/// Defines table structure, relationships, constraints, and indexes.
/// TODO: User to consider additional indexes for HR/manager approval queries
/// </summary>
public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");

        // Primary Key
        builder.HasKey(lr => lr.Id);

        // Properties
        builder.Property(lr => lr.EmployeeId).IsRequired();
        builder.Property(lr => lr.LeaveType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(lr => lr.StartDate).IsRequired();
        builder.Property(lr => lr.EndDate).IsRequired();

        builder.Property(lr => lr.NumberOfDays).IsRequired();

        builder.Property(lr => lr.DaysDeducted)
            .HasPrecision(5, 2)
            .HasDefaultValue(0);

        // Reason
        builder.Property(lr => lr.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(lr => lr.ReplacementEmployeeId)
            .HasMaxLength(100);

        // Approval Workflow
        builder.Property(lr => lr.RequestStatus)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Pending");

        // Manager Approval
        builder.Property(lr => lr.ManagerId);
        builder.Property(lr => lr.ManagerApprovedAt);

        builder.Property(lr => lr.ManagerRemarks)
            .HasMaxLength(500);

        // HR Approval
        builder.Property(lr => lr.HRApproverId);
        builder.Property(lr => lr.HRApprovedAt);

        builder.Property(lr => lr.HRRemarks)
            .HasMaxLength(500);

        // Additional Info
        builder.Property(lr => lr.AttachmentUrl)
            .HasMaxLength(500);

        builder.Property(lr => lr.HalfDayPeriod)
            .HasMaxLength(20);

        builder.Property(lr => lr.IsHalfDay)
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(lr => lr.Employee)
            .WithMany() // TODO: Consider adding ICollection<LeaveRequest> to Employee if needed
            .HasForeignKey(lr => lr.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(lr => lr.EmployeeId);
        builder.HasIndex(lr => lr.RequestStatus); // For HR/Manager approval queries
        builder.HasIndex(lr => new { lr.EmployeeId, lr.StartDate }); // For leave history
        builder.HasIndex(lr => new { lr.ManagerId, lr.RequestStatus }).HasDatabaseName("IX_LeaveRequests_ManagerStatus");
        builder.HasIndex(lr => new { lr.HRApproverId, lr.RequestStatus }).HasDatabaseName("IX_LeaveRequests_HRStatus");

        // Audit Fields (inherited from AggregateRoot)
        builder.Property(lr => lr.CreatedAt).IsRequired();
        builder.Property(lr => lr.UpdatedAt);
        builder.Property(lr => lr.IsDeleted).HasDefaultValue(false);
    }
}
