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
    /// TODO: Implement check-in validation and duplicate prevention
    /// </summary>
    public async Task<AttendanceLog> CheckInAsync(Guid employeeId, DateTime checkInTime, string? location = null)
    {
        // TODO: User to implement:
        // - Validate employee is active
        // - Check no duplicate check-in today
        // - Validate location if required
        // - Geofencing if applicable
        // - Store check-in event

        var id = Guid.NewGuid();
        var log = new AttendanceLog(employeeId, checkInTime.Date);
        log.Id = id;
        log.CheckInTime = checkInTime;
        log.Location = location;
        log.AttendanceStatus = "Present";

        log.CheckIn(checkInTime);
        _attendanceLogs[id] = log;

        return await Task.FromResult(log);
    }

    /// <summary>
    /// Record employee check-out and calculate worked hours.
    /// TODO: Implement check-out validation and hour calculation
    /// </summary>
    public async Task<AttendanceLog> CheckOutAsync(Guid employeeId, DateTime checkOutTime, string? remarks = null)
    {
        // TODO: User to implement:
        // - Find today's check-in for employee
        // - Validate check-out is after check-in
        // - Calculate worked hours with break deduction
        // - Detect early leave (if applicable)
        // - Update attendance status
        // - Store check-out event

        var today = checkOutTime.Date;
        var log = _attendanceLogs.Values.FirstOrDefault(a => a.EmployeeId == employeeId && a.AttendanceDate == today);

        if (log == null)
            throw new KeyNotFoundException($"No check-in found for employee {employeeId} on {today}");

        log.CheckOut(checkOutTime);
        log.CalculateWorkedHours();
        log.Remarks = remarks;

        return await Task.FromResult(log);
    }

    /// <summary>
    /// Get attendance log for a specific date.
    /// TODO: Add authorization and soft-delete handling
    /// </summary>
    public async Task<AttendanceLog?> GetAttendanceByDateAsync(Guid employeeId, DateTime date)
    {
        // TODO: User to implement:
        // - Database query with soft-delete filter
        // - Timezone normalization
        // - Caching if frequent access

        var log = _attendanceLogs.Values.FirstOrDefault(a => a.EmployeeId == employeeId && a.AttendanceDate == date);
        return await Task.FromResult(log);
    }

    /// <summary>
    /// Get attendance logs for a date range.
    /// TODO: Add pagination, filtering, authorization
    /// </summary>
    public async Task<List<AttendanceLog>> GetAttendanceByRangeAsync(Guid employeeId, DateTime startDate, DateTime endDate)
    {
        // TODO: User to implement:
        // - Database query with date range filter
        // - Pagination support
        // - Sorting options
        // - Soft-delete filtering
        // - Caching for performance

        var logs = _attendanceLogs.Values
            .Where(a => a.EmployeeId == employeeId && a.AttendanceDate >= startDate && a.AttendanceDate <= endDate)
            .OrderBy(a => a.AttendanceDate)
            .ToList();

        return await Task.FromResult(logs);
    }

    /// <summary>
    /// Approve/adjust attendance (for HR/Manager override).
    /// TODO: Implement approval workflow with authorization
    /// </summary>
    public async Task<AttendanceLog> ApproveAttendanceAsync(Guid employeeId, DateTime date, string status, Guid approverId, string remarks)
    {
        // TODO: User to implement:
        // - Validate approver is HR or employee's manager
        // - Validate date is not future
        // - Allow status adjustment (late, absent, etc.)
        // - Recalculate if needed
        // - Store approval audit

        var log = _attendanceLogs.Values.FirstOrDefault(a => a.EmployeeId == employeeId && a.AttendanceDate == date);

        if (log == null)
        {
            log = new AttendanceLog(employeeId, date);
            log.Id = Guid.NewGuid();
            _attendanceLogs[log.Id] = log;
        }

        log.ApproveAttendance(approverId, remarks);
        log.AttendanceStatus = status;

        return await Task.FromResult(log);
    }

    /// <summary>
    /// Get summary of attendance for a period (present, absent, late, etc.).
    /// TODO: Implement aggregation and reporting
    /// </summary>
    public async Task<AttendanceSummary> GetAttendanceSummaryAsync(Guid employeeId, DateTime startDate, DateTime endDate)
    {
        // TODO: User to implement:
        // - Aggregate attendance records
        // - Calculate totals: present, absent, late, etc.
        // - Calculate average worked hours
        // - Generate report data
        // - Cache summary for performance

        var logs = _attendanceLogs.Values
            .Where(a => a.EmployeeId == employeeId && a.AttendanceDate >= startDate && a.AttendanceDate <= endDate)
            .ToList();

        var summary = new AttendanceSummary
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate,
            TotalDays = (int)(endDate - startDate).TotalDays + 1,
            PresentDays = logs.Count(a => a.AttendanceStatus == "Present"),
            AbsentDays = logs.Count(a => a.AttendanceStatus == "Absent"),
            TotalWorkedHours = (decimal)logs.Sum(a => a.WorkedHours),
            AverageWorkedHours = logs.Count > 0 ? (decimal)logs.Average(a => a.WorkedHours) : 0
        };

        return await Task.FromResult(summary);
    }
}

/// <summary>
/// Summary statistics for attendance period.
/// TODO: Consider adding more metrics (half-day, leave, etc.)
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
