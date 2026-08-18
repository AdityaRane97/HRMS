namespace HRMS.Application.DTOs;

/// <summary>
/// DTO for LeaveRequest response (read-only API response).
/// </summary>
public class LeaveRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeFullName { get; set; } = string.Empty;

    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfDays { get; set; }
    public decimal DaysDeducted { get; set; }

    public string Reason { get; set; } = string.Empty;
    public string? ReplacementEmployeeId { get; set; }

    public string RequestStatus { get; set; } = "Pending";

    public Guid? ManagerId { get; set; }
    public DateTime? ManagerApprovedAt { get; set; }
    public string? ManagerRemarks { get; set; }

    public Guid? HRApproverId { get; set; }
    public DateTime? HRApprovedAt { get; set; }
    public string? HRRemarks { get; set; }

    public string? AttachmentUrl { get; set; }
    public bool IsHalfDay { get; set; }
    public string? HalfDayPeriod { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a leave request.
/// </summary>
public class CreateLeaveRequestDto
{
    public Guid EmployeeId { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;

    public string? ReplacementEmployeeId { get; set; }
    public string? AttachmentUrl { get; set; }
    public bool IsHalfDay { get; set; } = false;
    public string? HalfDayPeriod { get; set; } // FirstHalf or SecondHalf
}

/// <summary>
/// DTO for manager to approve or reject leave request.
/// </summary>
public class ApproveLeaveByManagerDto
{
    public Guid ManagerId { get; set; }
    public bool IsApproved { get; set; }
    public string Remarks { get; set; } = string.Empty;
}

/// <summary>
/// DTO for HR to approve or reject leave request (final decision).
/// </summary>
public class ApproveLeaveByHRDto
{
    public Guid HRApproverId { get; set; }
    public bool IsApproved { get; set; }
    public string Remarks { get; set; } = string.Empty;
}

/// <summary>
/// DTO for employee to cancel an approved leave.
/// </summary>
public class CancelLeaveDto
{
    public string CancellationReason { get; set; } = string.Empty;
}
