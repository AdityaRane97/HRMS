using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

/// <summary>
/// Entity Framework Core configuration for AuditLog entity.
/// Audit logs are required for GDPR, SOC2, and DPDP compliance with 3-year retention.
/// </summary>
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "dbo");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasDefaultValueSql("NEWID()");

        builder.Property(a => a.ActorId);
        builder.Property(a => a.ActorName).HasMaxLength(256);
        builder.Property(a => a.EntityType).HasMaxLength(100).IsRequired();
        builder.Property(a => a.EntityId).IsRequired();
        builder.Property(a => a.ActionType).HasMaxLength(50).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(1000);
        builder.Property(a => a.ChangesJson).HasColumnType("NVARCHAR(MAX)");
        builder.Property(a => a.Source).HasMaxLength(256);
        builder.Property(a => a.CorrelationId).HasMaxLength(256);
        builder.Property(a => a.Status).HasMaxLength(50).HasDefaultValue("Success");

        builder.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()").IsRequired();
        builder.Property(a => a.CreatedBy).HasMaxLength(256);

        // Indexes for audit queries and compliance reporting
        builder.HasIndex(a => a.EntityType);
        builder.HasIndex(a => a.EntityId);
        builder.HasIndex(a => a.ActionType);
        builder.HasIndex(a => a.CreatedAt); // For retention queries
        builder.HasIndex(a => a.ActorId);
        builder.HasIndex(a => new { a.EntityType, a.EntityId, a.CreatedAt }); // Composite for compliance reports
        builder.HasIndex(a => a.CorrelationId); // For tracking related actions
    }
}
