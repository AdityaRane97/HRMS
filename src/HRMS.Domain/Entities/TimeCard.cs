using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents a timecard for an employee covering a specific period (week, bi-weekly, monthly).
/// Contains multiple time entries and tracks submission/approval workflow.
/// </summary>
public class TimeCard : BaseEntity
{
    /// <summary>
    /// Foreign key to Employee
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Navigation property to Employee
    /// </summary>
    public virtual Employee? Employee { get; set; }

    /// <summary>
    /// Start date of the timecard period
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// End date of the timecard period
    /// </summary>
    public DateTime PeriodEnd { get; set; }

    /// <summary>
    /// Status of the timecard (Draft, Submitted, ApprovedByManager, ApprovedByHR, Rejected)
    /// </summary>
    public string Status { get; set; } = "Draft"; // Draft, Submitted, ApprovedByManager, ApprovedByHR, Rejected

    /// <summary>
    /// Total hours reported in this timecard
    /// </summary>
    public decimal TotalHours { get; set; }

    /// <summary>
    /// Date when the timecard was submitted
    /// </summary>
    public DateTime? SubmittedDate { get; set; }

    /// <summary>
    /// Employee ID of the manager who approved
    /// </summary>
    public Guid? ApprovedByManagerId { get; set; }

    /// <summary>
    /// Date when manager approved
    /// </summary>
    public DateTime? ApprovedByManagerDate { get; set; }

    /// <summary>
    /// Manager's remarks/comments
    /// </summary>
    public string? ManagerRemarks { get; set; }

    /// <summary>
    /// Employee ID of the HR person who approved
    /// </summary>
    public Guid? ApprovedByHRId { get; set; }

    /// <summary>
    /// Date when HR approved
    /// </summary>
    public DateTime? ApprovedByHRDate { get; set; }

    /// <summary>
    /// HR's remarks/comments
    /// </summary>
    public string? HRRemarks { get; set; }

    /// <summary>
    /// Reason for rejection if applicable
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// Navigation property for time entries
    /// </summary>
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = [];

    public TimeCard()
    {
    }

    public TimeCard(Guid employeeId, DateTime periodStart, DateTime periodEnd)
    {
        EmployeeId = employeeId;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
    }

    /// <summary>
    /// Calculate total hours from all time entries
    /// </summary>
    public void CalculateTotalHours()
    {
        TotalHours = TimeEntries.Where(e => e.Status != "Rejected").Sum(e => e.Hours);
    }

    /// <summary>
    /// Submit the timecard for approval
    /// </summary>
    public void Submit()
    {
        if (Status != "Draft")
            throw new InvalidOperationException("Only draft timecards can be submitted");

        Status = "Submitted";
        SubmittedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Approve by manager
    /// </summary>
    public void ApproveByManager(Guid managerId, string? remarks = null)
    {
        if (Status != "Submitted")
            throw new InvalidOperationException("Only submitted timecards can be approved");

        Status = "ApprovedByManager";
        ApprovedByManagerId = managerId;
        ApprovedByManagerDate = DateTime.UtcNow;
        ManagerRemarks = remarks;
    }

    /// <summary>
    /// Approve by HR
    /// </summary>
    public void ApproveByHR(Guid hrId, string? remarks = null)
    {
        if (Status != "ApprovedByManager")
            throw new InvalidOperationException("Timecard must be approved by manager first");

        Status = "ApprovedByHR";
        ApprovedByHRId = hrId;
        ApprovedByHRDate = DateTime.UtcNow;
        HRRemarks = remarks;
    }

    /// <summary>
    /// Reject the timecard
    /// </summary>
    public void Reject(string reason)
    {
        if (Status is not ("Submitted" or "ApprovedByManager"))
            throw new InvalidOperationException("Cannot reject timecard in current status");

        Status = "Rejected";
        RejectionReason = reason;
    }

    /// <summary>
    /// Check if timecard is locked (approved or submitted)
    /// </summary>
    public bool IsLocked() => Status is "ApprovedByManager" or "ApprovedByHR" or "Submitted";

    /// <summary>
    /// Check if timecard can be edited
    /// </summary>
    public bool CanEdit() => Status == "Draft";
}
