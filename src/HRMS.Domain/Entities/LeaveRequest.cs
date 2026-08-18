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
    /// TODO: Implement business day calculation logic
    /// Consider: Weekends, public holidays, half-day rules, overlapping leaves, etc.
    /// </summary>
    public void CalculateDaysDeducted()
    {
        // TODO: User to implement days calculation
        // Example logic seed:
        // var totalDays = (EndDate.Date - StartDate.Date).Days + 1; // Inclusive
        // var businessDays = 0;
        // for (var date = StartDate.Date; date <= EndDate.Date; date = date.AddDays(1))
        // {
        //     if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
        //         businessDays++;
        // }
        // DaysDeducted = IsHalfDay ? 0.5m : businessDays;
        // NumberOfDays = totalDays;

        throw new NotImplementedException("CalculateDaysDeducted must be implemented with your business rules");
    }

    /// <summary>
    /// Validate leave request before submission.
    /// TODO: Implement validation rules
    /// Consider: Date validation, leave balance check, overlapping leaves, approval workflow, etc.
    /// </summary>
    public void Validate()
    {
        // TODO: User to implement validation logic
        // Example logic seed:
        // if (StartDate > EndDate) throw new Exception("Start date must be before end date");
        // if (StartDate < DateTime.UtcNow.Date) throw new Exception("Cannot request leave for past dates");
        // if (string.IsNullOrEmpty(Reason)) throw new Exception("Reason is required");

        throw new NotImplementedException("Validate must be implemented with your business rules");
    }

    /// <summary>
    /// Submit leave request to manager.
    /// TODO: Implement request submission and notification logic
    /// </summary>
    public void Submit()
    {
        // TODO: User to implement submission logic
        // Example logic seed:
        // Validate();
        // RequestStatus = "Pending";
        // RaiseDomainEvent(new LeaveRequestSubmittedEvent(...));

        throw new NotImplementedException("Submit must be implemented");
    }

    /// <summary>
    /// Approve leave request by manager.
    /// TODO: Implement manager approval logic
    /// </summary>
    public void ApproveByManager(Guid managerId, string remarks = "")
    {
        // TODO: User to implement manager approval logic
        // Example logic seed:
        // if (RequestStatus != "Pending") throw new Exception("Invalid status for approval");
        // ManagerId = managerId;
        // ManagerApprovedAt = DateTime.UtcNow;
        // ManagerRemarks = remarks;
        // RequestStatus = "ApprovedByManager";
        // RaiseDomainEvent(new LeaveApprovedByManagerEvent(...));

        throw new NotImplementedException("ApproveByManager must be implemented");
    }

    /// <summary>
    /// Reject leave request by manager.
    /// TODO: Implement manager rejection logic
    /// </summary>
    public void RejectByManager(Guid managerId, string remarks)
    {
        // TODO: User to implement manager rejection logic
        // Example logic seed:
        // if (RequestStatus != "Pending") throw new Exception("Invalid status for rejection");
        // ManagerId = managerId;
        // ManagerRemarks = remarks;
        // RequestStatus = "RejectedByManager";
        // RaiseDomainEvent(new LeaveRejectedByManagerEvent(...));

        throw new NotImplementedException("RejectByManager must be implemented");
    }

    /// <summary>
    /// Approve leave request by HR (final approval).
    /// TODO: Implement HR approval logic
    /// </summary>
    public void ApproveByHR(Guid hrApproverId, string remarks = "")
    {
        // TODO: User to implement HR approval logic
        // Example logic seed:
        // if (RequestStatus != "ApprovedByManager") throw new Exception("Manager approval required first");
        // HRApproverId = hrApproverId;
        // HRApprovedAt = DateTime.UtcNow;
        // HRRemarks = remarks;
        // RequestStatus = "ApprovedByHR";
        // CalculateDaysDeducted(); // Calculate and deduct from balance
        // RaiseDomainEvent(new LeaveApprovedByHREvent(...));

        throw new NotImplementedException("ApproveByHR must be implemented");
    }

    /// <summary>
    /// Reject leave request by HR.
    /// TODO: Implement HR rejection logic
    /// </summary>
    public void RejectByHR(Guid hrApproverId, string remarks)
    {
        // TODO: User to implement HR rejection logic
        // Example logic seed:
        // if (RequestStatus != "ApprovedByManager") throw new Exception("Invalid status for HR rejection");
        // HRApproverId = hrApproverId;
        // HRRemarks = remarks;
        // RequestStatus = "RejectedByHR";
        // RaiseDomainEvent(new LeaveRejectedByHREvent(...));

        throw new NotImplementedException("RejectByHR must be implemented");
    }

    /// <summary>
    /// Cancel an approved leave request.
    /// TODO: Implement cancellation logic
    /// Consider: Restore leave balance, notify approvers, audit trail, etc.
    /// </summary>
    public void Cancel(string reason)
    {
        // TODO: User to implement cancellation logic
        // Example logic seed:
        // if (RequestStatus != "ApprovedByHR") throw new Exception("Can only cancel approved leaves");
        // if (StartDate <= DateTime.UtcNow.Date) throw new Exception("Cannot cancel ongoing/past leaves");
        // RequestStatus = "Cancelled";
        // RaiseDomainEvent(new LeaveCancelledEvent(...));

        throw new NotImplementedException("Cancel must be implemented");
    }

    /// <summary>
    /// Check if leave request is approved.
    /// </summary>
    public bool IsApproved() => RequestStatus == "ApprovedByHR";

    /// <summary>
    /// Check if leave request is pending.
    /// </summary>
    public bool IsPending() => RequestStatus == "Pending";

    /// <summary>
    /// Check if leave request is rejected.
    /// </summary>
    public bool IsRejected() => RequestStatus.Contains("Rejected");
}
