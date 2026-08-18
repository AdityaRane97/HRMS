using HRMS.Application.Services;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Services;

/// <summary>
/// In-memory leave management service for Phase 2 (placeholder; replace with database in Phase 3).
/// Handles leave request lifecycle: submit, approve/reject, cancel.
/// TODO: Implement leave balance tracking, workflow engine in Phase 3+
/// </summary>
public class InMemoryLeaveService : ILeaveService
{
    private readonly Dictionary<Guid, LeaveRequest> _leaveRequests = new();

    /// <summary>
    /// Create and submit a leave request.
    /// TODO: Implement validation, leave balance check, workflow initiation
    /// </summary>
    public async Task<LeaveRequest> SubmitLeaveRequestAsync(Guid employeeId, string leaveType, DateTime startDate, DateTime endDate, string reason)
    {
        // TODO: User to implement:
        // - Validate employee exists and is active
        // - Check leave balance for leave type
        // - Check for overlapping leave requests
        // - Validate advance notice requirement
        // - Calculate days deducted
        // - Assign to manager for approval

        var id = Guid.NewGuid();
        var request = new LeaveRequest(employeeId, leaveType, startDate, endDate, reason);
        request.Id = id;

        request.Validate();
        request.Submit();
        _leaveRequests[id] = request;

        return await Task.FromResult(request);
    }

    /// <summary>
    /// Get leave request by ID.
    /// TODO: Add authorization checks
    /// </summary>
    public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(Guid leaveRequestId)
    {
        // TODO: User to implement:
        // - Database query
        // - Authorization: employee, manager, or HR can view
        // - Soft-delete filtering

        _leaveRequests.TryGetValue(leaveRequestId, out var request);
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Get all leave requests for an employee (optionally filtered by status).
    /// TODO: Add pagination, sorting, status filtering
    /// </summary>
    public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeAsync(Guid employeeId, string? status = null)
    {
        // TODO: User to implement:
        // - Database query
        // - Filter by status if provided
        // - Sort by date descending
        // - Pagination support
        // - Soft-delete filtering

        var requests = _leaveRequests.Values
            .Where(lr => lr.EmployeeId == employeeId && (status == null || lr.RequestStatus == status))
            .OrderByDescending(lr => lr.StartDate)
            .ToList();

        return await Task.FromResult(requests);
    }

    /// <summary>
    /// Approve leave request by manager.
    /// TODO: Implement manager-specific workflow, notifications
    /// </summary>
    public async Task<LeaveRequest> ApproveByManagerAsync(Guid leaveRequestId, Guid managerId, string remarks = "")
    {
        // TODO: User to implement:
        // - Check manager is actually the employee's manager
        // - Check leave is in "Pending" status
        // - Validate manager can approve this leave type
        // - Send notification to employee
        // - Send to HR for final approval
        // - Audit log

        var request = _leaveRequests.Values.FirstOrDefault(lr => lr.Id == leaveRequestId);
        if (request == null)
            throw new KeyNotFoundException($"Leave request {leaveRequestId} not found");

        request.ApproveByManager(managerId, remarks);
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Reject leave request by manager.
    /// TODO: Implement rejection workflow, notifications
    /// </summary>
    public async Task<LeaveRequest> RejectByManagerAsync(Guid leaveRequestId, Guid managerId, string remarks)
    {
        // TODO: User to implement:
        // - Validate manager authority
        // - Check leave status is "Pending"
        // - Remarks validation (required for rejection)
        // - Notify employee of rejection
        // - No leave balance deduction

        var request = _leaveRequests.Values.FirstOrDefault(lr => lr.Id == leaveRequestId);
        if (request == null)
            throw new KeyNotFoundException($"Leave request {leaveRequestId} not found");

        request.RejectByManager(managerId, remarks);
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Approve leave request by HR (final approval).
    /// TODO: Implement HR-specific workflow, balance deduction, final notifications
    /// </summary>
    public async Task<LeaveRequest> ApproveByHRAsync(Guid leaveRequestId, Guid hrApproverId, string remarks = "")
    {
        // TODO: User to implement:
        // - Check HR authority
        // - Check status is "ApprovedByManager"
        // - Calculate and deduct days from leave balance
        // - Update employee's leave balance table
        // - Notify all parties
        // - Lock leave request from amendments
        // - Audit trail with approval timestamp

        var request = _leaveRequests.Values.FirstOrDefault(lr => lr.Id == leaveRequestId);
        if (request == null)
            throw new KeyNotFoundException($"Leave request {leaveRequestId} not found");

        request.ApproveByHR(hrApproverId, remarks);
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Reject leave request by HR.
    /// TODO: Implement HR rejection, final notifications
    /// </summary>
    public async Task<LeaveRequest> RejectByHRAsync(Guid leaveRequestId, Guid hrApproverId, string remarks)
    {
        // TODO: User to implement:
        // - Check HR authority
        // - Check status is "ApprovedByManager"
        // - Remarks validation (required)
        // - Notify employee
        // - No balance deduction

        var request = _leaveRequests.Values.FirstOrDefault(lr => lr.Id == leaveRequestId);
        if (request == null)
            throw new KeyNotFoundException($"Leave request {leaveRequestId} not found");

        request.RejectByHR(hrApproverId, remarks);
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Cancel an approved leave request.
    /// TODO: Implement cancellation workflow, balance restoration
    /// </summary>
    public async Task<LeaveRequest> CancelLeaveAsync(Guid leaveRequestId, string cancellationReason)
    {
        // TODO: User to implement:
        // - Check status is "ApprovedByHR"
        // - Check start date is in future
        // - Restore leave balance
        // - Notify manager and HR
        // - Audit trail

        var request = _leaveRequests.Values.FirstOrDefault(lr => lr.Id == leaveRequestId);
        if (request == null)
            throw new KeyNotFoundException($"Leave request {leaveRequestId} not found");

        request.Cancel(cancellationReason);
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Get pending leave requests for manager (leaves awaiting manager approval).
    /// TODO: Add filtering by department, urgency, etc.
    /// </summary>
    public async Task<List<LeaveRequest>> GetPendingLeavesForManagerAsync(Guid managerId)
    {
        // TODO: User to implement:
        // - Query employees managed by managerId
        // - Get leave requests from those employees with status "Pending"
        // - Sort by start date (urgent first)
        // - Pagination

        var pending = _leaveRequests.Values
            .Where(lr => lr.RequestStatus == "Pending")
            .OrderBy(lr => lr.StartDate)
            .ToList();

        return await Task.FromResult(pending);
    }

    /// <summary>
    /// Get pending leave requests for HR.
    /// TODO: Add filtering, sorting, prioritization
    /// </summary>
    public async Task<List<LeaveRequest>> GetPendingLeavesForHRAsync()
    {
        // TODO: User to implement:
        // - Query all leaves with status "ApprovedByManager"
        // - Sort by start date or submission date
        // - Pagination
        // - Dashboard metrics (pending, approved, rejected)

        var pending = _leaveRequests.Values
            .Where(lr => lr.RequestStatus == "ApprovedByManager")
            .OrderBy(lr => lr.StartDate)
            .ToList();

        return await Task.FromResult(pending);
    }
}
