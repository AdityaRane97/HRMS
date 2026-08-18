using HRMS.Domain.Entities;

namespace HRMS.Application.Services;

/// <summary>
/// Service interface for payroll management.
/// Defines operations for payroll creation, calculation, and payment processing.
/// TODO: Phase 3 will implement database-backed version with tax calculation engine
/// </summary>
public interface IPayrollService
{
    Task<PayrollRecord> CreatePayrollAsync(Guid employeeId, DateTime payrollMonth, decimal baseSalary);
    Task<PayrollRecord?> GetPayrollByIdAsync(Guid payrollId);
    Task<List<PayrollRecord>> GetPayrollByEmployeeAsync(Guid employeeId, DateTime startMonth, DateTime endMonth);
    Task<PayrollRecord> UpdatePayrollAsync(Guid payrollId, Action<PayrollRecord> updateAction);
    Task<PayrollRecord> ProcessPayrollAsync(Guid payrollId);
    Task<PayrollRecord> MarkAsPaidAsync(Guid payrollId, string referenceNumber);
    Task<List<PayrollRecord>> GetPendingPayrollsAsync();
}

/// <summary>
/// Service interface for attendance management.
/// Defines operations for check-in/out and attendance tracking.
/// TODO: Phase 3+ will integrate biometric systems, face recognition, geofencing
/// </summary>
public interface IAttendanceService
{
    Task<AttendanceLog> CheckInAsync(Guid employeeId, DateTime checkInTime, string? location = null);
    Task<AttendanceLog> CheckOutAsync(Guid employeeId, DateTime checkOutTime, string? remarks = null);
    Task<AttendanceLog?> GetAttendanceByDateAsync(Guid employeeId, DateTime date);
    Task<List<AttendanceLog>> GetAttendanceByRangeAsync(Guid employeeId, DateTime startDate, DateTime endDate);
    Task<AttendanceLog> ApproveAttendanceAsync(Guid employeeId, DateTime date, string status, Guid approverId, string remarks);
    // TODO: Add method for bulk attendance approval (by department, date range)
    // TODO: Add method for geofencing validation
}

/// <summary>
/// Service interface for leave management.
/// Defines operations for leave request lifecycle.
/// TODO: Phase 3 will implement leave balance tracking, carry-forward rules, accrual calculations
/// </summary>
public interface ILeaveService
{
    Task<LeaveRequest> SubmitLeaveRequestAsync(Guid employeeId, string leaveType, DateTime startDate, DateTime endDate, string reason);
    Task<LeaveRequest?> GetLeaveRequestByIdAsync(Guid leaveRequestId);
    Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeAsync(Guid employeeId, string? status = null);
    Task<LeaveRequest> ApproveByManagerAsync(Guid leaveRequestId, Guid managerId, string remarks = "");
    Task<LeaveRequest> RejectByManagerAsync(Guid leaveRequestId, Guid managerId, string remarks);
    Task<LeaveRequest> ApproveByHRAsync(Guid leaveRequestId, Guid hrApproverId, string remarks = "");
    Task<LeaveRequest> RejectByHRAsync(Guid leaveRequestId, Guid hrApproverId, string remarks);
    Task<LeaveRequest> CancelLeaveAsync(Guid leaveRequestId, string cancellationReason);
    Task<List<LeaveRequest>> GetPendingLeavesForManagerAsync(Guid managerId);
    Task<List<LeaveRequest>> GetPendingLeavesForHRAsync();
    // TODO: Add method for leave balance query
    // TODO: Add method for leave policy configuration
    // TODO: Add method for leave calendar (blocked dates, public holidays)
}
