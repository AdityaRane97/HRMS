using AutoMapper;
using HRMS.Application.Constants;
using HRMS.Application.DTOs;
using HRMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers;

/// <summary>
/// API endpoints for attendance tracking and management.
/// Phase 2.3: Self-service endpoints use the authenticated user's identity.
/// Resource-based authorization for read endpoints will be added separately.
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
    /// Record employee check-in.
    /// The employee identity is taken from the authenticated JWT user.
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

        var result = await _attendanceService.CheckInAsync(
            currentUserId.Value,
            dto.CheckInTime,
            dto.Location);

        return Ok(_mapper.Map<AttendanceLogDto>(result));
    }

    /// <summary>
    /// Record employee check-out.
    /// The employee identity is taken from the authenticated JWT user.
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

        var result = await _attendanceService.CheckOutAsync(
            currentUserId.Value,
            dto.CheckOutTime,
            dto.Remarks);

        return Ok(_mapper.Map<AttendanceLogDto>(result));
    }

    /// <summary>
    /// Get attendance for a specific date.
    /// Resource-level authorization will be added in a later Batch.
    /// </summary>
    [HttpGet("{employeeId:guid}/{date:datetime}")]
    public async Task<ActionResult<AttendanceLogDto>> GetAttendanceByDate(
        Guid employeeId,
        DateTime date)
    {
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
    /// Resource-level authorization will be added in a later Batch.
    /// </summary>
    [HttpGet("{employeeId:guid}/range")]
    public async Task<ActionResult<List<AttendanceLogDto>>>
        GetAttendanceByRange(
            Guid employeeId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
    {
        var results =
            await _attendanceService.GetAttendanceByRangeAsync(
                employeeId,
                startDate,
                endDate);

        return Ok(_mapper.Map<List<AttendanceLogDto>>(results));
    }

    /// <summary>
    /// Approve or adjust attendance.
    /// Accessible by Manager, HR, and Admin.
    /// The approver identity is taken from the authenticated JWT user.
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

        var result =
            await _attendanceService.ApproveAttendanceAsync(
                dto.EmployeeId,
                dto.AttendanceDate,
                dto.AttendanceStatus,
                currentUserId.Value,
                dto.ApprovalRemarks);

        return Ok(_mapper.Map<AttendanceLogDto>(result));
    }

    /// <summary>
    /// Get attendance summary for a period.
    /// Resource-level authorization will be added in a later Batch.
    /// </summary>
    [HttpGet("{employeeId:guid}/summary")]
    public async Task<IActionResult> GetAttendanceSummary(
        Guid employeeId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var summary = new
        {
            employeeId,
            startDate,
            endDate
        };

        return Ok(summary);
    }

    private Guid? TryGetCurrentUserId()
    {
        return _currentUserService.UserId;
    }
}