using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Data.Config;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        // Table configuration
        builder.ToTable("TimeEntries");

        // Primary key
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntryDate)
            .IsRequired();

        builder.Property(x => x.Project)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Task)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Hours)
            .HasPrecision(5, 2); // Max 999.99 hours

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Draft");

        builder.Property(x => x.Comments)
            .HasMaxLength(500);

        // Relationships
        builder.HasOne(x => x.TimeCard)
            .WithMany(x => x.TimeEntries)
            .HasForeignKey(x => x.TimeCardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(x => x.TimeCardId)
            .HasName("ix_time_entries_timecard_id");

        builder.HasIndex(x => x.EntryDate)
            .HasName("ix_time_entries_date");

        builder.HasIndex(x => new { x.TimeCardId, x.EntryDate })
            .HasName("ix_time_entries_timecard_date");

        // Soft delete support from BaseEntity
        builder.Property<DateTime?>("DeletedAt");
        builder.Property<bool>("IsDeleted").HasDefaultValue(false);
    }
}
