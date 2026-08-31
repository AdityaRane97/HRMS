using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents a time entry (tracked work hours) for project/task work.
/// Multiple time entries belong to a TimeCard for a specific period.
/// </summary>
public class TimeEntry : BaseEntity
{
    /// <summary>
    /// Foreign key to TimeCard
    /// </summary>
    public Guid TimeCardId { get; set; }

    /// <summary>
    /// Navigation property to TimeCard
    /// </summary>
    public virtual TimeCard? TimeCard { get; set; }

    /// <summary>
    /// Date of the work entry
    /// </summary>
    public DateTime EntryDate { get; set; }

    /// <summary>
    /// Project name or ID
    /// </summary>
    public string Project { get; set; } = string.Empty;

    /// <summary>
    /// Task or activity name
    /// </summary>
    public string Task { get; set; } = string.Empty;

    /// <summary>
    /// Description of work performed
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Number of hours worked
    /// </summary>
    public decimal Hours { get; set; }

    /// <summary>
    /// Start time (optional, for detailed tracking)
    /// </summary>
    public TimeSpan? StartTime { get; set; }

    /// <summary>
    /// End time (optional, for detailed tracking)
    /// </summary>
    public TimeSpan? EndTime { get; set; }

    /// <summary>
    /// Status of the time entry (Draft, Submitted, Approved)
    /// </summary>
    public string Status { get; set; } = "Draft"; // Draft, Submitted, Approved, Rejected

    /// <summary>
    /// Comments or remarks about the entry
    /// </summary>
    public string? Comments { get; set; }

    public TimeEntry()
    {
    }

    public TimeEntry(Guid timeCardId, DateTime entryDate, string project, string task, decimal hours)
    {
        TimeCardId = timeCardId;
        EntryDate = entryDate;
        Project = project;
        Task = task;
        Hours = hours;
    }

    /// <summary>
    /// Validate that hours are within acceptable range
    /// </summary>
    public bool IsValid()
    {
        // Hours must be positive and not exceed 24
        if (Hours <= 0 || Hours > 24)
            return false;

        // Date cannot be in the future
        if (EntryDate.Date > DateTime.UtcNow.Date)
            return false;

        // If start and end times provided, they must be valid
        if (StartTime.HasValue && EndTime.HasValue && StartTime >= EndTime)
            return false;

        return true;
    }

    /// <summary>
    /// Calculate hours from start and end times if provided
    /// </summary>
    public void CalculateHoursFromTimes()
    {
        if (StartTime.HasValue && EndTime.HasValue && EndTime > StartTime)
        {
            var duration = EndTime.Value - StartTime.Value;
            Hours = (decimal)duration.TotalHours;
        }
    }
}
