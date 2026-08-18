using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents a leave request filed by an employee.
/// Supports various leave types (annual, sick, personal, etc.) with approval workflow.
/// </summary>
public class LeaveRequest : AggregateRoot
{
    // Employee Reference
    public Guid EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    // Leave Details
    public string LeaveType { get; set; } = string.Empty; // Annual, Sick, PersonalCare, Maternity, Paternity, Sabbatical, Unpaid, etc.
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfDays { get; set; } // Days (including partial days)
    public decimal DaysDeducted { get; set; } = 0; // Can be fractional for half-days

    // Reason
    public string Reason { get; set; } = string.Empty;
    public string? ReplacementEmployeeId { get; set; } // Who will cover tasks during leave

    // Approval Workflow
    public string RequestStatus { get; set; } = "Pending"; // Pending, ApprovedByManager, RejectedByManager, ApprovedByHR, RejectedByHR, Cancelled, Expired

    // Manager Approval
    public Guid? ManagerId { get; set; } // Manager who approves
    public DateTime? ManagerApprovedAt { get; set; }
    public string? ManagerRemarks { get; set; }

    // HR Approval
    public Guid? HRApproverId { get; set; } // HR who approves
    public DateTime? HRApprovedAt { get; set; }
    public string? HRRemarks { get; set; }

    // Additional Info
    public string? AttachmentUrl { get; set; } // Medical certificate, etc.
    public bool IsHalfDay { get; set; } = false;
    public string? HalfDayPeriod { get; set; } // FirstHalf, SecondHalf

    public LeaveRequest()
    {
    }

    public LeaveRequest(Guid employeeId, string leaveType, DateTime startDate, DateTime endDate, string reason)
    {
        EmployeeId = employeeId;
        LeaveType = leaveType;
        StartDate = startDate;
        EndDate = endDate;
        Reason = reason;
    }

    /// <summary>
    /// Calculate number of days for the leave request.
    /// Phase 2.1: Simple calendar-day count (no weekend/holiday filtering yet).
    /// Future: Add business-day calculation and holiday handling.
    /// </summary>
    public void CalculateDaysDeducted()
    {
        // Calendar days (inclusive of start and end dates)
        var totalDays = (EndDate.Date - StartDate.Date).Days + 1;

        // Apply half-day rule
        DaysDeducted = IsHalfDay ? 0.5m : totalDays;
        NumberOfDays = totalDays;
    }

    /// <summary>
    /// Validate leave request before submission.
    /// Phase 2.1: Basic date and reason validation.
    /// Future: Add leave balance checks, overlapping leave detection.
    /// </summary>
    public void Validate()
    {
        if (StartDate > EndDate)
            throw new InvalidOperationException("Start date must be before or equal to end date");

        if (StartDate.Date < DateTime.UtcNow.Date)
            throw new InvalidOperationException("Cannot request leave for past dates");

        if (string.IsNullOrWhiteSpace(Reason))
            throw new InvalidOperationException("Reason is required for leave request");
    }

    /// <summary>
    /// Submit leave request to manager.
    /// Validates and sets status to Pending.
    /// </summary>
    public void Submit()
    {
        Validate();
        RequestStatus = "Pending";
    }

    /// <summary>
    /// Approve leave request by manager.
    /// Manager must approve before HR review.
    /// </summary>
    public void ApproveByManager(Guid managerId, string remarks = "")
    {
        if (RequestStatus != "Pending")
            throw new InvalidOperationException("Only pending leave requests can be approved by manager");

        ManagerId = managerId;
        ManagerApprovedAt = DateTime.UtcNow;
        ManagerRemarks = remarks;
        RequestStatus = "ApprovedByManager";
    }

    /// <summary>
    /// Reject leave request by manager.
    /// Ends approval process without HR review.
    /// </summary>
    public void RejectByManager(Guid managerId, string remarks)
    {
        if (RequestStatus != "Pending")
            throw new InvalidOperationException("Only pending leave requests can be rejected by manager");

        ManagerId = managerId;
        ManagerRemarks = remarks;
        RequestStatus = "RejectedByManager";
    }

    /// <summary>
    /// Approve leave request by HR (final approval).
    /// Can only approve if manager already approved.
    /// Calculates and deducts days from leave balance.
    /// </summary>
    public void ApproveByHR(Guid hrApproverId, string remarks = "")
    {
        if (RequestStatus != "ApprovedByManager")
            throw new InvalidOperationException("Manager approval is required before HR approval");

        HRApproverId = hrApproverId;
        HRApprovedAt = DateTime.UtcNow;
        HRRemarks = remarks;
        RequestStatus = "ApprovedByHR";

        // Calculate days deducted upon final approval
        CalculateDaysDeducted();
    }

    /// <summary>
    /// Reject leave request by HR.
    /// Can only reject after manager approval.
    /// </summary>
    public void RejectByHR(Guid hrApproverId, string remarks)
    {
        if (RequestStatus != "ApprovedByManager")
            throw new InvalidOperationException("Invalid status for HR rejection");

        HRApproverId = hrApproverId;
        HRRemarks = remarks;
        RequestStatus = "RejectedByHR";
    }

    /// <summary>
    /// Cancel an approved leave request.
    /// Can only cancel if leave is approved and hasn't started yet.
    /// </summary>
    public void Cancel(string reason)
    {
        if (StartDate.Date <= DateTime.UtcNow.Date)
            throw new InvalidOperationException("Cannot cancel leave that has already started");

        if (RequestStatus != "ApprovedByHR")
            throw new InvalidOperationException("Only approved leave can be cancelled");

        RequestStatus = "Cancelled";
        Reason = $"Cancelled: {reason}";
    }

    /// <summary>
    /// Check if leave request is approved (final HR approval).
    /// </summary>
    public bool IsApproved() => RequestStatus == "ApprovedByHR";

    /// <summary>
    /// Check if leave request is pending manager approval.
    /// </summary>
    public bool IsPending() => RequestStatus == "Pending";

    /// <summary>
    /// Check if leave request is rejected by manager or HR.
    /// </summary>
    public bool IsRejected() => RequestStatus.Contains("Rejected");
}
