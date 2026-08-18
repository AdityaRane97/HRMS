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
    /// Validates, submits, and stores in in-memory collection.
    /// </summary>
    public async Task<LeaveRequest> SubmitLeaveRequestAsync(Guid employeeId, string leaveType, DateTime startDate, DateTime endDate, string reason)
    {
        var id = Guid.NewGuid();
        var request = new LeaveRequest(employeeId, leaveType, startDate, endDate, reason)
        {
            Id = id,
            CreatedAt = DateTime.UtcNow
        };

        request.Validate();
        request.Submit();
        _leaveRequests[id] = request;

        return await Task.FromResult(request);
    }

    /// <summary>
    /// Get leave request by ID.
    /// Returns null if not found.
    /// </summary>
    public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(Guid leaveRequestId)
    {
        _leaveRequests.TryGetValue(leaveRequestId, out var request);
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Get all leave requests for an employee, optionally filtered by status.
    /// Sorted by start date descending (newest first).
    /// </summary>
    public async Task<List<LeaveRequest>> GetLeaveRequestsByEmployeeAsync(Guid employeeId, string? status = null)
    {
        var requests = _leaveRequests.Values
            .Where(lr => lr.EmployeeId == employeeId 
                && (status == null || lr.RequestStatus == status))
            .OrderByDescending(lr => lr.StartDate)
            .ToList();

        return await Task.FromResult(requests);
    }

    /// <summary>
    /// Approve leave request by manager.
    /// Calls domain method to transition status.
    /// </summary>
    public async Task<LeaveRequest> ApproveByManagerAsync(Guid leaveRequestId, Guid managerId, string remarks = "")
    {
        var request = _leaveRequests.Values.FirstOrDefault(lr => lr.Id == leaveRequestId);
        if (request == null)
            throw new KeyNotFoundException($"Leave request {leaveRequestId} not found");

        request.ApproveByManager(managerId, remarks);
        request.UpdatedAt = DateTime.UtcNow;
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Reject leave request by manager.
    /// Ends approval process without HR review.
    /// </summary>
    public async Task<LeaveRequest> RejectByManagerAsync(Guid leaveRequestId, Guid managerId, string remarks)
    {
        if (string.IsNullOrWhiteSpace(remarks))
            throw new InvalidOperationException("Rejection remarks are required");

        var request = _leaveRequests.Values.FirstOrDefault(lr => lr.Id == leaveRequestId);
        if (request == null)
            throw new KeyNotFoundException($"Leave request {leaveRequestId} not found");

        request.RejectByManager(managerId, remarks);
        request.UpdatedAt = DateTime.UtcNow;
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Approve leave request by HR (final approval).
    /// Calculates days deducted and marks as approved.
    /// </summary>
    public async Task<LeaveRequest> ApproveByHRAsync(Guid leaveRequestId, Guid hrApproverId, string remarks = "")
    {
        var request = _leaveRequests.Values.FirstOrDefault(lr => lr.Id == leaveRequestId);
        if (request == null)
            throw new KeyNotFoundException($"Leave request {leaveRequestId} not found");

        request.ApproveByHR(hrApproverId, remarks);
        request.UpdatedAt = DateTime.UtcNow;
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Reject leave request by HR.
    /// Can only reject after manager approval.
    /// </summary>
    public async Task<LeaveRequest> RejectByHRAsync(Guid leaveRequestId, Guid hrApproverId, string remarks)
    {
        if (string.IsNullOrWhiteSpace(remarks))
            throw new InvalidOperationException("Rejection remarks are required");

        var request = _leaveRequests.Values.FirstOrDefault(lr => lr.Id == leaveRequestId);
        if (request == null)
            throw new KeyNotFoundException($"Leave request {leaveRequestId} not found");

        request.RejectByHR(hrApproverId, remarks);
        request.UpdatedAt = DateTime.UtcNow;
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Cancel an approved leave request.
    /// Can only cancel before leave start date.
    /// </summary>
    public async Task<LeaveRequest> CancelLeaveAsync(Guid leaveRequestId, string cancellationReason)
    {
        var request = _leaveRequests.Values.FirstOrDefault(lr => lr.Id == leaveRequestId);
        if (request == null)
            throw new KeyNotFoundException($"Leave request {leaveRequestId} not found");

        request.Cancel(cancellationReason);
        request.UpdatedAt = DateTime.UtcNow;
        return await Task.FromResult(request);
    }

    /// <summary>
    /// Get pending leave requests for manager.
    /// Returns leaves awaiting manager approval, sorted by start date.
    /// </summary>
    public async Task<List<LeaveRequest>> GetPendingLeavesForManagerAsync(Guid managerId)
    {
        var pending = _leaveRequests.Values
            .Where(lr => lr.RequestStatus == "Pending")
            .OrderBy(lr => lr.StartDate)
            .ToList();

        return await Task.FromResult(pending);
    }

    /// <summary>
    /// Get pending leave requests for HR.
    /// Returns leaves awaiting HR approval (already approved by manager).
    /// </summary>
    public async Task<List<LeaveRequest>> GetPendingLeavesForHRAsync()
    {
        var pending = _leaveRequests.Values
            .Where(lr => lr.RequestStatus == "ApprovedByManager")
            .OrderBy(lr => lr.StartDate)
            .ToList();

        return await Task.FromResult(pending);
    }
}
