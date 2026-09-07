using HRMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Infrastructure.Data.Config;

/// <summary>
/// Entity Framework Core configuration for the Document entity
/// Defines table structure, relationships, and constraints
/// </summary>
public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents");

        builder.HasKey(d => d.Id);

        // Properties
        builder.Property(d => d.EmployeeId)
            .IsRequired();

        builder.Property(d => d.DocumentName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.Category)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.Description)
            .HasMaxLength(2000);

        builder.Property(d => d.FileName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.FileType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.FileSize)
            .IsRequired();

        builder.Property(d => d.UploadDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(d => d.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.DocumentUrl)
            .HasMaxLength(2000);

        builder.Property(d => d.UploadedByEmail)
            .HasMaxLength(256);

        builder.Property(d => d.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(d => d.UpdatedAt);

        builder.Property(d => d.UpdatedByEmail)
            .HasMaxLength(256);

        builder.Property(d => d.UploadedByUserId);

        // Relationships
        builder.HasOne(d => d.Employee)
            .WithMany()
            .HasForeignKey(d => d.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(d => d.EmployeeId);
        builder.HasIndex(d => d.Category);
        builder.HasIndex(d => d.UploadDate);
        builder.HasIndex(d => d.IsActive);
        builder.HasIndex(d => new { d.EmployeeId, d.IsActive });
    }
}
