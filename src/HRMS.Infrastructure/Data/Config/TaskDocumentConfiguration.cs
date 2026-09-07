using HRMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Infrastructure.Data.Config;

/// <summary>
/// Entity Framework Core configuration for the TaskDocument entity
/// Defines the join table between Tasks and Documents
/// </summary>
public class TaskDocumentConfiguration : IEntityTypeConfiguration<TaskDocument>
{
    public void Configure(EntityTypeBuilder<TaskDocument> builder)
    {
        builder.ToTable("TaskDocuments");

        builder.HasKey(td => td.Id);

        // Properties
        builder.Property(td => td.TaskId)
            .IsRequired();

        builder.Property(td => td.DocumentId)
            .IsRequired();

        builder.Property(td => td.DocumentPurpose)
            .HasMaxLength(200);

        builder.Property(td => td.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(td => td.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(td => td.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(td => td.UpdatedAt);

        // Relationships
        builder.HasOne(td => td.Task)
            .WithMany(t => t.TaskDocuments)
            .HasForeignKey(td => td.TaskId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TaskDocuments_Tasks");

        builder.HasOne(td => td.Document)
            .WithMany()
            .HasForeignKey(td => td.DocumentId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_TaskDocuments_Documents");

        // Indexes
        builder.HasIndex(td => td.TaskId);
        builder.HasIndex(td => td.DocumentId);
        builder.HasIndex(td => new { td.TaskId, td.IsPrimary });
        builder.HasIndex(td => td.IsDeleted);
    }
}
