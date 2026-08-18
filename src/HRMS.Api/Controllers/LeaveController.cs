using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using HRMS.Application.DTOs;
using HRMS.Application.Services;

namespace HRMS.Api.Controllers;

/// <summary>
/// API endpoints for leave request management.
/// Phase 2.2: Protected with [Authorize]. Resource-based and role-based checks to be added in Phase 3.
/// TODO: Add role-based authorization (own request, manager, HR), workflow notifications, leave balance validation
/// </summary>
[Authorize]
[ApiController]
[Route("api/leave")]
[Tags("Leave")]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;
    private readonly IMapper _mapper;
    private readonly ILogger<LeaveController> _logger;

    public LeaveController(ILeaveService leaveService, IMapper mapper, ILogger<LeaveController> logger)
    {
        _leaveService = leaveService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Submit a new leave request.
    /// TODO: Add leave balance validation, notification to manager
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> SubmitLeaveRequest([FromBody] CreateLeaveRequestDto dto)
    {
        // TODO: User to implement:
        // - Validate employee exists
        // - Check leave balance
        // - Validate advance notice
        // - Call service
        // - Send notification to manager

        var result = await _leaveService.SubmitLeaveRequestAsync(
            dto.EmployeeId,
            dto.LeaveType,
            dto.StartDate,
            dto.EndDate,
            dto.Reason
        );

        return CreatedAtAction(nameof(GetLeaveRequestById), new { id = result.Id }, _mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Get leave request by ID.
    /// TODO: Add authorization (own request, manager, HR)
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequestById(Guid id)
    {
        // TODO: User to implement:
        // - Check authorization
        // - Call service
        // - Handle not found

        var result = await _leaveService.GetLeaveRequestByIdAsync(id);
        if (result == null)
            return NotFound();

        return Ok(_mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Get all leave requests for employee.
    /// TODO: Add filtering by status, pagination
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetLeaveRequestsByEmployee(Guid employeeId, [FromQuery] string? status = null)
    {
        // TODO: User to implement:
        // - Check authorization
        // - Add pagination
        // - Add sorting

        var results = await _leaveService.GetLeaveRequestsByEmployeeAsync(employeeId, status);
        return Ok(_mapper.Map<List<LeaveRequestDto>>(results));
    }

    /// <summary>
    /// Manager approves leave request.
    /// TODO: Add manager authorization, notification to employee and HR
    /// </summary>
    [HttpPost("{id:guid}/approve-manager")]
    public async Task<ActionResult<LeaveRequestDto>> ApproveByManager(Guid id, [FromBody] ApproveLeaveByManagerDto dto)
    {
        // TODO: User to implement:
        // - Check user is the employee's manager
        // - Validate leave status is "Pending"
        // - Call service based on IsApproved flag
        // - Send notifications

        var result = dto.IsApproved
            ? await _leaveService.ApproveByManagerAsync(id, dto.ManagerId, dto.Remarks)
            : await _leaveService.RejectByManagerAsync(id, dto.ManagerId, dto.Remarks);

        return Ok(_mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// HR approves leave request (final approval).
    /// TODO: Add HR authorization, balance deduction, notifications
    /// </summary>
    [HttpPost("{id:guid}/approve-hr")]
    public async Task<ActionResult<LeaveRequestDto>> ApproveByHR(Guid id, [FromBody] ApproveLeaveByHRDto dto)
    {
        // TODO: User to implement:
        // - Check user is HR
        // - Validate leave status is "ApprovedByManager"
        // - Call service based on IsApproved flag
        // - Deduct from employee leave balance
        // - Send final notifications

        var result = dto.IsApproved
            ? await _leaveService.ApproveByHRAsync(id, dto.HRApproverId, dto.Remarks)
            : await _leaveService.RejectByHRAsync(id, dto.HRApproverId, dto.Remarks);

        return Ok(_mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Employee cancels an approved leave.
    /// TODO: Add authorization (must be own leave), restoration of balance
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<LeaveRequestDto>> CancelLeave(Guid id, [FromBody] CancelLeaveDto dto)
    {
        // TODO: User to implement:
        // - Check user owns the leave
        // - Validate leave status is "ApprovedByHR"
        // - Check start date is future
        // - Call service
        // - Restore leave balance
        // - Notify manager/HR

        var result = await _leaveService.CancelLeaveAsync(id, dto.CancellationReason);
        return Ok(_mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Get pending leave requests for manager.
    /// TODO: Add pagination, filtering by urgency/department
    /// </summary>
    [HttpGet("pending-for-manager/{managerId:guid}")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetPendingLeavesForManager(Guid managerId)
    {
        // TODO: User to implement:
        // - Check user is the manager
        // - Add pagination
        // - Add filtering options
        // - Add caching

        var results = await _leaveService.GetPendingLeavesForManagerAsync(managerId);
        return Ok(_mapper.Map<List<LeaveRequestDto>>(results));
    }

    /// <summary>
    /// Get pending leave requests for HR.
    /// TODO: Add pagination, status filtering, dashboard metrics
    /// </summary>
    [HttpGet("pending-for-hr")]
    public async Task<ActionResult<List<LeaveRequestDto>>> GetPendingLeavesForHR()
    {
        // TODO: User to implement:
        // - Check user is HR
        // - Add pagination
        // - Add filtering
        // - Add dashboard summary (pending count, urgent, by department)

        var results = await _leaveService.GetPendingLeavesForHRAsync();
        return Ok(_mapper.Map<List<LeaveRequestDto>>(results));
    }
}
