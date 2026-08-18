namespace HRMS.Application.DTOs;

/// <summary>
/// DTO for AttendanceLog response (read-only API response).
/// </summary>
public class AttendanceLogDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeFullName { get; set; } = string.Empty;
    public DateTime AttendanceDate { get; set; }

    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }

    public decimal WorkedHours { get; set; }
    public string AttendanceStatus { get; set; } = "Present";

    public string? Location { get; set; }
    public string? Remarks { get; set; }

    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalRemarks { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for check-in (employee marks attendance start).
/// </summary>
public class AttendanceCheckInDto
{
    public Guid EmployeeId { get; set; }
    public DateTime CheckInTime { get; set; }
    public string? Location { get; set; }
}

/// <summary>
/// DTO for check-out (employee marks attendance end).
/// </summary>
public class AttendanceCheckOutDto
{
    public Guid EmployeeId { get; set; }
    public DateTime CheckOutTime { get; set; }
    public string? Remarks { get; set; }
}

/// <summary>
/// DTO for HR/Manager to approve or adjust attendance.
/// TODO: User to define approval workflow (mark late, mark absent, override status, etc.)
/// </summary>
public class AttendanceApprovalDto
{
    public Guid EmployeeId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public string AttendanceStatus { get; set; } = string.Empty; // Present, Absent, LeaveApproved, HalfDay, etc.
    public decimal? WorkedHours { get; set; } // Manual override if needed
    public string ApprovalRemarks { get; set; } = string.Empty;
}
