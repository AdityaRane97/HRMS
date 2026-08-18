using HRMS.Application.Services;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Services;

/// <summary>
/// In-memory attendance service for Phase 2 (placeholder; replace with biometric/database in Phase 3+).
/// Handles check-in/out, worked hour calculation, and attendance approval.
/// TODO: Implement biometric integration and persistent storage in Phase 3+
/// </summary>
public class InMemoryAttendanceService : IAttendanceService
{
    private readonly Dictionary<Guid, AttendanceLog> _attendanceLogs = new();

    /// <summary>
    /// Record employee check-in.
    /// Creates attendance log and records check-in time.
    /// </summary>
    public async Task<AttendanceLog> CheckInAsync(Guid employeeId, DateTime checkInTime, string? location = null)
    {
        var today = checkInTime.Date;

        // Ensure no duplicate check-in for today
        if (_attendanceLogs.Values.Any(a => a.EmployeeId == employeeId && a.AttendanceDate == today))
            throw new InvalidOperationException($"Employee {employeeId} has already checked in for {today}");

        var id = Guid.NewGuid();
        var log = new AttendanceLog(employeeId, today)
        {
            Id = id,
            Location = location,
            CreatedAt = DateTime.UtcNow
        };

        log.CheckIn(checkInTime);
        _attendanceLogs[id] = log;

        return await Task.FromResult(log);
    }

    /// <summary>
    /// Record employee check-out and calculate worked hours.
    /// Calls domain CheckOut which triggers CalculateWorkedHours.
    /// </summary>
    public async Task<AttendanceLog> CheckOutAsync(Guid employeeId, DateTime checkOutTime, string? remarks = null)
    {
        var today = checkOutTime.Date;
        var log = _attendanceLogs.Values.FirstOrDefault(a => a.EmployeeId == employeeId && a.AttendanceDate == today);

        if (log == null)
            throw new KeyNotFoundException($"No check-in found for employee {employeeId} on {today}");

        log.CheckOut(checkOutTime);
        log.Remarks = remarks;
        log.UpdatedAt = DateTime.UtcNow;

        return await Task.FromResult(log);
    }

    /// <summary>
    /// Get attendance log for a specific date.
    /// Returns null if not found.
    /// </summary>
    public async Task<AttendanceLog?> GetAttendanceByDateAsync(Guid employeeId, DateTime date)
    {
        var log = _attendanceLogs.Values.FirstOrDefault(a => a.EmployeeId == employeeId && a.AttendanceDate == date);
        return await Task.FromResult(log);
    }

    /// <summary>
    /// Get attendance logs for a date range.
    /// Sorted by attendance date ascending.
    /// </summary>
    public async Task<List<AttendanceLog>> GetAttendanceByRangeAsync(Guid employeeId, DateTime startDate, DateTime endDate)
    {
        var logs = _attendanceLogs.Values
            .Where(a => a.EmployeeId == employeeId 
                && a.AttendanceDate >= startDate.Date 
                && a.AttendanceDate <= endDate.Date)
            .OrderBy(a => a.AttendanceDate)
            .ToList();

        return await Task.FromResult(logs);
    }

    /// <summary>
    /// Approve/adjust attendance (for HR/Manager override).
    /// Creates attendance record if not exists, then approves.
    /// </summary>
    public async Task<AttendanceLog> ApproveAttendanceAsync(Guid employeeId, DateTime date, string status, Guid approverId, string remarks)
    {
        var log = _attendanceLogs.Values.FirstOrDefault(a => a.EmployeeId == employeeId && a.AttendanceDate == date.Date);

        if (log == null)
        {
            log = new AttendanceLog(employeeId, date.Date)
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            };
            _attendanceLogs[log.Id] = log;
        }

        log.ApproveAttendance(approverId, remarks);
        log.AttendanceStatus = status;
        log.UpdatedAt = DateTime.UtcNow;

        return await Task.FromResult(log);
    }

    /// <summary>
    /// Get summary of attendance for a period.
    /// Aggregates present, absent days and total/average worked hours.
    /// </summary>
    public async Task<AttendanceSummary> GetAttendanceSummaryAsync(Guid employeeId, DateTime startDate, DateTime endDate)
    {
        var logs = _attendanceLogs.Values
            .Where(a => a.EmployeeId == employeeId 
                && a.AttendanceDate >= startDate.Date 
                && a.AttendanceDate <= endDate.Date)
            .ToList();

        var summary = new AttendanceSummary
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate,
            TotalDays = (int)(endDate.Date - startDate.Date).TotalDays + 1,
            PresentDays = logs.Count(a => a.AttendanceStatus == "Present"),
            AbsentDays = logs.Count(a => a.AttendanceStatus == "Absent"),
            TotalWorkedHours = (decimal)logs.Sum(a => (double)a.WorkedHours),
            AverageWorkedHours = logs.Count > 0 ? (decimal)logs.Average(a => (double)a.WorkedHours) : 0
        };

        return await Task.FromResult(summary);
    }
}

/// <summary>
/// Summary statistics for attendance period.
/// Includes present/absent days and worked hours metrics.
/// </summary>
public class AttendanceSummary
{
    public Guid EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public int PresentDays { get; set; }
    public int AbsentDays { get; set; }
    public decimal TotalWorkedHours { get; set; }
    public decimal AverageWorkedHours { get; set; }
}
