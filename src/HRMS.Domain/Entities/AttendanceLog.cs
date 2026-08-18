using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents an attendance log entry for an employee.
/// Tracks daily attendance, check-in/out times, and status.
/// </summary>
public class AttendanceLog : BaseEntity
{
    // Employee Reference
    public Guid EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    // Attendance Date & Time
    public DateTime AttendanceDate { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }

    // Computed Fields
    public decimal WorkedHours { get; set; } = 0; // Calculated from CheckIn/CheckOut
    public string AttendanceStatus { get; set; } = "Present"; // Present, Absent, LeaveApproved, HalfDay, LateArrival, EarlyLeave, WFH (Work From Home)

    // Additional Info
    public string? Location { get; set; } // Office, Remote, OnSite, etc.
    public string? Remarks { get; set; } // Late reason, approval reference, etc.

    // Manager/HR Override
    public Guid? ApprovedBy { get; set; } // HR/Manager ID if manually adjusted
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalRemarks { get; set; }

    public AttendanceLog()
    {
    }

    public AttendanceLog(Guid employeeId, DateTime attendanceDate)
    {
        EmployeeId = employeeId;
        AttendanceDate = attendanceDate;
    }

    /// <summary>
    /// Record check-in time.
    /// Phase 2.1: Simple logging; no duplicate check-in prevention yet.
    /// </summary>
    public void CheckIn(DateTime checkInTime)
    {
        if (CheckInTime.HasValue)
            throw new InvalidOperationException("Employee has already checked in for this date");

        CheckInTime = checkInTime;
        AttendanceStatus = "Present";
    }

    /// <summary>
    /// Record check-out time and calculate worked hours.
    /// Validates checkout time is after check-in.
    /// </summary>
    public void CheckOut(DateTime checkOutTime)
    {
        if (!CheckInTime.HasValue)
            throw new InvalidOperationException("Employee has not checked in yet");

        if (checkOutTime < CheckInTime.Value)
            throw new InvalidOperationException("Check-out time cannot be before check-in time");

        CheckOutTime = checkOutTime;
        CalculateWorkedHours();
    }

    /// <summary>
    /// Calculate worked hours from CheckInTime and CheckOutTime.
    /// Phase 2.1: Simple total - default 1-hour break deduction (lunch).
    /// Future: Configurable break rules.
    /// </summary>
    public void CalculateWorkedHours()
    {
        if (CheckInTime.HasValue && CheckOutTime.HasValue)
        {
            var totalMinutes = (CheckOutTime.Value - CheckInTime.Value).TotalMinutes;
            var breakMinutes = 60; // Default 1-hour lunch break
            var workedMinutes = totalMinutes - breakMinutes;
            WorkedHours = (decimal)workedMinutes / 60;

            // Ensure non-negative
            if (WorkedHours < 0)
                WorkedHours = 0;
        }
    }

    /// <summary>
    /// Mark attendance as approved by HR/Manager.
    /// Used for manual overrides or exceptions.
    /// </summary>
    public void ApproveAttendance(Guid approverId, string remarks = "")
    {
        ApprovedBy = approverId;
        ApprovedAt = DateTime.UtcNow;
        ApprovalRemarks = remarks;
    }

    /// <summary>
    /// Check if attendance is late.
    /// Summary>
    /// Check if attendance is late.
    /// Phase 2.1: Deferred (user chose no late detection yet).
    /// Returns false for now.
    /// </summary>
    public bool IsLateArrival()
    {
        // Phase 2.1: No late detection
        return false;
    }
}
