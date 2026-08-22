using AutoMapper;
using HRMS.Application.Constants;
using HRMS.Application.DTOs;
using HRMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/leave")]
[Tags("Leave")]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<LeaveController> _logger;

    public LeaveController(
        ILeaveService leaveService,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<LeaveController> logger)
    {
        _leaveService = leaveService;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Gets the currently authenticated user ID from the JWT claims.
    /// </summary>
    private Guid? TryGetCurrentUserId()
    {
        return _currentUserService.UserId;
    }

    /// <summary>
    /// Submit a leave request.
    /// The employee ID is always taken from the authenticated JWT.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LeaveRequestDto>> CreateLeaveRequest(
        [FromBody] CreateLeaveRequestDto dto)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        var result = await _leaveService.SubmitLeaveRequestAsync(
            currentUserId.Value,
            dto.LeaveType,
            dto.StartDate,
            dto.EndDate,
            dto.Reason);

        return CreatedAtAction(
            nameof(GetLeaveRequestById),
            new { id = result.Id },
            _mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Get leave request by ID.
    /// Employees can access only their own requests.
    /// HR and Admin can access any request.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LeaveRequestDto>> GetLeaveRequestById(
        Guid id)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        var result =
            await _leaveService.GetLeaveRequestByIdAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        var canAccess =
            result.EmployeeId == currentUserId.Value ||
            _currentUserService.IsInRole(RoleConstants.HR) ||
            _currentUserService.IsInRole(RoleConstants.Admin);

        if (!canAccess)
        {
            return Forbid();
        }

        return Ok(_mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Get leave requests for an employee.
    /// Employees can access only their own requests.
    /// HR and Admin can access any employee's requests.
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>>
        GetLeaveRequestsByEmployee(
            Guid employeeId,
            [FromQuery] string? status = null)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        var canAccess =
            employeeId == currentUserId.Value ||
            _currentUserService.IsInRole(RoleConstants.HR) ||
            _currentUserService.IsInRole(RoleConstants.Admin);

        if (!canAccess)
        {
            return Forbid();
        }

        var result =
            await _leaveService.GetLeaveRequestsByEmployeeAsync(
                employeeId,
                status);

        return Ok(_mapper.Map<List<LeaveRequestDto>>(result));
    }

    /// <summary>
    /// Approve leave request by manager.
    /// The approver ID is always taken from the authenticated JWT.
    /// </summary>
    [Authorize(Roles = RoleConstants.ManagerHROrAdmin)]
    [HttpPost("{id:guid}/approve-manager")]
    public async Task<ActionResult<LeaveRequestDto>> ApproveByManager(
        Guid id,
        [FromBody] ApproveLeaveByManagerDto dto)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        var result =
            await _leaveService.ApproveByManagerAsync(
                id,
                currentUserId.Value,
                dto.Remarks ?? string.Empty);

        return Ok(_mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Reject leave request by manager.
    /// The approver ID is always taken from the authenticated JWT.
    /// </summary>
    [Authorize(Roles = RoleConstants.ManagerHROrAdmin)]
    [HttpPost("{id:guid}/reject-manager")]
    public async Task<ActionResult<LeaveRequestDto>> RejectByManager(
        Guid id,
        [FromBody] ApproveLeaveByManagerDto dto)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        var result =
            await _leaveService.RejectByManagerAsync(
                id,
                currentUserId.Value,
                dto.Remarks ?? string.Empty);

        return Ok(_mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Approve leave request by HR.
    /// The approver ID is always taken from the authenticated JWT.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpPost("{id:guid}/approve-hr")]
    public async Task<ActionResult<LeaveRequestDto>> ApproveByHr(
        Guid id,
        [FromBody] ApproveLeaveByHRDto dto)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        var result =
            await _leaveService.ApproveByHRAsync(
                id,
                currentUserId.Value,
                dto.Remarks ?? string.Empty);

        return Ok(_mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Reject leave request by HR.
    /// The approver ID is always taken from the authenticated JWT.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpPost("{id:guid}/reject-hr")]
    public async Task<ActionResult<LeaveRequestDto>> RejectByHr(
        Guid id,
        [FromBody] ApproveLeaveByHRDto dto)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        var result =
            await _leaveService.RejectByHRAsync(
                id,
                currentUserId.Value,
                dto.Remarks ?? string.Empty);

        return Ok(_mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Cancel a leave request.
    /// Employees can cancel only their own leave requests.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<ActionResult<LeaveRequestDto>> CancelLeaveRequest(
        Guid id,
        [FromBody] CancelLeaveDto dto)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        var leaveRequest =
            await _leaveService.GetLeaveRequestByIdAsync(id);

        if (leaveRequest == null)
        {
            return NotFound();
        }

        var canCancel =
            leaveRequest.EmployeeId == currentUserId.Value ||
            _currentUserService.IsInRole(RoleConstants.HR) ||
            _currentUserService.IsInRole(RoleConstants.Admin);

        if (!canCancel)
        {
            return Forbid();
        }

        var result =
            await _leaveService.CancelLeaveAsync(
                id,
                dto.CancellationReason);

        return Ok(_mapper.Map<LeaveRequestDto>(result));
    }

    /// <summary>
    /// Get leave requests pending for a manager.
    /// A manager can access only their own pending requests.
    /// HR and Admin can query any manager.
    /// </summary>
    [Authorize(Roles = RoleConstants.ManagerHROrAdmin)]
    [HttpGet("pending-for-manager/{managerId:guid}")]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>>
        GetPendingForManager(Guid managerId)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        var isPrivilegedUser =
            _currentUserService.IsInRole(RoleConstants.HR) ||
            _currentUserService.IsInRole(RoleConstants.Admin);

        if (managerId != currentUserId.Value && !isPrivilegedUser)
        {
            return Forbid();
        }

        var result =
            await _leaveService.GetPendingLeavesForManagerAsync(
                managerId);

        return Ok(_mapper.Map<List<LeaveRequestDto>>(result));
    }

    /// <summary>
    /// Get leave requests pending HR approval.
    /// Accessible only by HR and Admin.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpGet("pending-for-hr")]
    public async Task<ActionResult<IEnumerable<LeaveRequestDto>>>
        GetPendingForHr()
    {
        var result =
            await _leaveService.GetPendingLeavesForHRAsync();

        return Ok(_mapper.Map<List<LeaveRequestDto>>(result));
    }
}