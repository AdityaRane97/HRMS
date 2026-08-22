using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using HRMS.Application.Constants;
using HRMS.Application.DTOs;
using HRMS.Application.Services;
using HRMS.Domain.Entities;

namespace HRMS.Api.Controllers;

/// <summary>
/// API endpoints for payroll management.
/// Phase 2.3: Role-Based and resource-level authorization applied.
/// </summary>
[Authorize]
[ApiController]
[Route("api/payroll")]
[Tags("Payroll")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<PayrollController> _logger;

    public PayrollController(
        IPayrollService payrollService,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<PayrollController> logger)
    {
        _payrollService = payrollService;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Gets the currently authenticated user ID from JWT claims.
    /// </summary>
    private Guid? TryGetCurrentUserId()
    {
        return _currentUserService.UserId;
    }

    /// <summary>
    /// Determines whether the current user has privileged payroll access.
    /// HR and Admin can access payroll records for any employee.
    /// </summary>
    private bool HasPrivilegedPayrollAccess()
    {
        return _currentUserService.IsInRole(RoleConstants.HR) ||
               _currentUserService.IsInRole(RoleConstants.Admin);
    }

    /// <summary>
    /// Create new payroll record for an employee.
    /// HR and Admin only.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpPost]
    public async Task<ActionResult<PayrollRecordDto>> CreatePayroll(
        [FromBody] CreatePayrollRecordDto dto)
    {
        var result = await _payrollService.CreatePayrollAsync(
            dto.EmployeeId,
            dto.PayrollMonth,
            dto.BaseSalary);

        var response = _mapper.Map<PayrollRecordDto>(result);

        return CreatedAtAction(
            nameof(GetPayrollById),
            new { id = response.Id },
            response);
    }

    /// <summary>
    /// Get payroll record by ID.
    /// Employees can access only their own payroll.
    /// HR and Admin can access any payroll record.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PayrollRecordDto>> GetPayrollById(Guid id)
    {
        var currentUserId = TryGetCurrentUserId();

        if (currentUserId is null)
        {
            return Unauthorized(new
            {
                message = "Invalid authenticated user."
            });
        }

        var payroll = await _payrollService.GetPayrollByIdAsync(id);

        if (payroll == null)
        {
            return NotFound();
        }

        var canAccess =
            payroll.EmployeeId == currentUserId.Value ||
            HasPrivilegedPayrollAccess();

        if (!canAccess)
        {
            return Forbid();
        }

        return Ok(_mapper.Map<PayrollRecordDto>(payroll));
    }

    /// <summary>
    /// Get payroll records for an employee in a date range.
    /// Employees can access only their own payroll records.
    /// HR and Admin can access any employee's payroll records.
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<List<PayrollRecordDto>>> GetPayrollByEmployee(
        Guid employeeId,
        [FromQuery] DateTime? startMonth,
        [FromQuery] DateTime? endMonth)
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
            HasPrivilegedPayrollAccess();

        if (!canAccess)
        {
            return Forbid();
        }

        startMonth ??= DateTime.UtcNow.AddMonths(-3);
        endMonth ??= DateTime.UtcNow;

        var payrolls = await _payrollService.GetPayrollByEmployeeAsync(
            employeeId,
            startMonth.Value,
            endMonth.Value);

        return Ok(_mapper.Map<List<PayrollRecordDto>>(payrolls));
    }

    /// <summary>
    /// Update payroll record.
    /// HR and Admin only.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PayrollRecordDto>> UpdatePayroll(
        Guid id,
        [FromBody] UpdatePayrollRecordDto dto)
    {
        var updated = await _payrollService.UpdatePayrollAsync(
            id,
            payroll =>
            {
                if (dto.PaymentDate.HasValue)
                {
                    payroll.PaymentDate = dto.PaymentDate.Value;
                }

                if (dto.HouseRentAllowance.HasValue)
                {
                    payroll.HouseRentAllowance =
                        dto.HouseRentAllowance.Value;
                }

                if (dto.MedicalAllowance.HasValue)
                {
                    payroll.MedicalAllowance =
                        dto.MedicalAllowance.Value;
                }
            });

        return Ok(_mapper.Map<PayrollRecordDto>(updated));
    }

    /// <summary>
    /// Process payroll.
    /// HR and Admin only.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpPost("{id:guid}/process")]
    public async Task<ActionResult<PayrollRecordDto>> ProcessPayroll(Guid id)
    {
        var processed = await _payrollService.ProcessPayrollAsync(id);

        return Ok(_mapper.Map<PayrollRecordDto>(processed));
    }

    /// <summary>
    /// Mark payroll as paid.
    /// HR and Admin only.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpPost("{id:guid}/pay")]
    public async Task<ActionResult<PayrollRecordDto>> MarkAsPaid(
        Guid id,
        [FromBody] string referenceNumber)
    {
        var paid = await _payrollService.MarkAsPaidAsync(
            id,
            referenceNumber);

        return Ok(_mapper.Map<PayrollRecordDto>(paid));
    }

    /// <summary>
    /// Get all pending payroll records.
    /// HR and Admin only.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpGet("pending")]
    public async Task<ActionResult<List<PayrollRecordDto>>>
        GetPendingPayrolls()
    {
        var pending = await _payrollService.GetPendingPayrollsAsync();

        return Ok(_mapper.Map<List<PayrollRecordDto>>(pending));
    }
}