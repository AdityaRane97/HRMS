using HRMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = HRMS.Domain.Entities.TaskModel;

namespace HRMS.Infrastructure.Data.Config;

/// <summary>
/// Entity Framework Core configuration for the Task entity
/// Defines table structure, relationships, and constraints for task management
/// </summary>
public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.Description)
            .HasMaxLength(4000);

        builder.Property(t => t.SprintName)
            .HasMaxLength(100);

        builder.Property(t => t.Category)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("General");

        builder.Property(t => t.Priority)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Medium");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("To Do");

        builder.Property(t => t.StartDate)
            .IsRequired();

        builder.Property(t => t.DueDate)
            .IsRequired();

        builder.Property(t => t.Notes)
            .HasMaxLength(4000);

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.CompletedDate);

        builder.Property(t => t.AssignedEmployeeId)
            .IsRequired();

        builder.Property(t => t.CreatedByEmployeeId)
            .IsRequired();

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationships
        builder.HasOne(t => t.AssignedEmployee)
            .WithMany()
            .HasForeignKey(t => t.AssignedEmployeeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Tasks_Employees_Assigned");

        builder.HasOne(t => t.CreatedByEmployee)
            .WithMany()
            .HasForeignKey(t => t.CreatedByEmployeeId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("FK_Tasks_Employees_CreatedBy");

        // Task-TaskDocument one-to-many relationship
        builder.HasMany(t => t.TaskDocuments)
            .WithOne(td => td.Task)
            .HasForeignKey(td => td.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for common queries
        builder.HasIndex(t => t.AssignedEmployeeId);
        builder.HasIndex(t => t.CreatedByEmployeeId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.DueDate);
        builder.HasIndex(t => t.IsDeleted);
        builder.HasIndex(t => new { t.AssignedEmployeeId, t.Status, t.IsDeleted });
        builder.HasIndex(t => new { t.Status, t.DueDate, t.IsDeleted });
    }
}
