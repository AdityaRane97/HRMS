using AutoMapper;
using HRMS.Application.Constants;
using HRMS.Application.DTOs;
using HRMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers;

/// <summary>
/// API endpoints for attendance tracking and management.
/// Phase 2.3: Role-Based Access Control + Resource-Level Authorization.
/// Self-service endpoints enforce employee identity from JWT, not from request body.
/// Read endpoints validate ownership: own record OR Manager/HR/Admin.
/// </summary>
[Authorize]
[ApiController]
[Route("api/attendance")]
[Tags("Attendance")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(
        IAttendanceService attendanceService,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<AttendanceController> logger)
    {
        _attendanceService = attendanceService;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Helper: Get current authenticated user ID or return 401.
    /// </summary>
    private Guid? TryGetCurrentUserId()
    {
        return _currentUserService.UserId;
    }

    /// <summary>
    /// Helper: Check if current user can access another employee's attendance record.
    /// Allowed if: own record OR Manager/HR/Admin.
    /// </summary>
    private bool CanAccessEmployeeRecord(Guid targetEmployeeId)
    {
        if (_currentUserService.UserId == targetEmployeeId)
            return true; // Own record

        if (_currentUserService.IsInRole(RoleConstants.Manager) ||
            _currentUserService.IsInRole(RoleConstants.HR) ||
            _currentUserService.IsInRole(RoleConstants.Admin))
            return true; // Manager/HR/Admin can access all

        return false; // Unauthorized
    }

    /// <summary>
    /// Record employee check-in.
    /// Self-service: Employee can only check in for themselves (uses JWT identity, not DTO).
    /// </summary>
    [HttpPost("check-in")]
    public async Task<ActionResult<AttendanceLogDto>> CheckIn(
        [FromBody] AttendanceCheckInDto dto)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        _logger.LogInformation($"Employee {currentUserId} checking in at {dto.CheckInTime}");

        var result = await _attendanceService.CheckInAsync(
            currentUserId.Value, // Use JWT identity, ignore DTO
            dto.CheckInTime,
            dto.Location);

        return Ok(_mapper.Map<AttendanceLogDto>(result));
    }

    /// <summary>
    /// Record employee check-out.
    /// Self-service: Employee can only check out for themselves (uses JWT identity, not DTO).
    /// </summary>
    [HttpPost("check-out")]
    public async Task<ActionResult<AttendanceLogDto>> CheckOut(
        [FromBody] AttendanceCheckOutDto dto)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        _logger.LogInformation($"Employee {currentUserId} checking out at {dto.CheckOutTime}");

        var result = await _attendanceService.CheckOutAsync(
            currentUserId.Value, // Use JWT identity, ignore DTO
            dto.CheckOutTime,
            dto.Remarks);

        return Ok(_mapper.Map<AttendanceLogDto>(result));
    }

    /// <summary>
    /// Get attendance for a specific employee and date.
    /// Resource-level authorization: own record OR Manager/HR/Admin.
    /// </summary>
    [HttpGet("{employeeId:guid}/{date:datetime}")]
    public async Task<ActionResult<AttendanceLogDto>> GetAttendanceByDate(
        Guid employeeId,
        DateTime date)
    {
        // Phase 2.3: Resource-level authorization check
        if (!CanAccessEmployeeRecord(employeeId))
        {
            _logger.LogWarning($"User {_currentUserService.UserId} attempted unauthorized access to employee {employeeId} attendance.");
            return Forbid();
        }

        var result = await _attendanceService.GetAttendanceByDateAsync(
            employeeId,
            date);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<AttendanceLogDto>(result));
    }

    /// <summary>
    /// Get attendance records for a date range.
    /// Resource-level authorization: own records OR Manager/HR/Admin.
    /// </summary>
    [HttpGet("{employeeId:guid}/range")]
    public async Task<ActionResult<List<AttendanceLogDto>>> GetAttendanceByRange(
        Guid employeeId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Phase 2.3: Resource-level authorization check
        if (!CanAccessEmployeeRecord(employeeId))
        {
            _logger.LogWarning($"User {_currentUserService.UserId} attempted unauthorized access to employee {employeeId} attendance range.");
            return Forbid();
        }

        if (startDate > endDate)
        {
            return BadRequest(new
            {
                message = "Start date cannot be greater than end date."
            });
        }

        var results = await _attendanceService.GetAttendanceByRangeAsync(
            employeeId,
            startDate,
            endDate);

        return Ok(_mapper.Map<List<AttendanceLogDto>>(results));
    }

    /// <summary>
    /// Approve or adjust attendance.
    /// Manager, HR, Admin only. Approver identity from JWT (not DTO).
    /// </summary>
    [Authorize(Roles = RoleConstants.ManagerHROrAdmin)]
    [HttpPost("approve")]
    public async Task<ActionResult<AttendanceLogDto>> ApproveAttendance(
        [FromBody] AttendanceApprovalDto dto)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        _logger.LogInformation($"User {currentUserId} approving attendance for employee {dto.EmployeeId}");

        var result = await _attendanceService.ApproveAttendanceAsync(
            dto.EmployeeId,
            dto.AttendanceDate,
            dto.AttendanceStatus,
            currentUserId.Value, // Use JWT identity, ignore DTO
            dto.ApprovalRemarks);

        return Ok(_mapper.Map<AttendanceLogDto>(result));
    }

    /// <summary>
    /// Get attendance summary for a period.
    /// Resource-level authorization: own summary OR Manager/HR/Admin.
    /// </summary>
    [HttpGet("{employeeId:guid}/summary")]
    public async Task<IActionResult> GetAttendanceSummary(
        Guid employeeId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Phase 2.3: Resource-level authorization check
        if (!CanAccessEmployeeRecord(employeeId))
        {
            _logger.LogWarning($"User {_currentUserService.UserId} attempted unauthorized access to employee {employeeId} summary.");
            return Forbid();
        }

        if (startDate > endDate)
        {
            return BadRequest(new
            {
                message = "Start date cannot be greater than end date."
            });
        }

        var attendanceRecords = await _attendanceService.GetAttendanceByRangeAsync(
            employeeId,
            startDate,
            endDate);

        var summary = new
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate,
            TotalRecords = attendanceRecords.Count,
            PresentCount = attendanceRecords.Count(a => a.AttendanceStatus == "Present"),
            ApprovedLeaveCount = attendanceRecords.Count(a => a.AttendanceStatus == "LeaveApproved"),
            AbsentCount = attendanceRecords.Count(a => a.AttendanceStatus == "Absent")
        };

        return Ok(summary);
    }
}
