using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using HRMS.Application.DTOs;
using HRMS.Application.Services;

namespace HRMS.Api.Controllers;

/// <summary>
/// API endpoints for payroll management.
/// TODO: Add authorization checks (HR only), caching, pagination
/// </summary>
[ApiController]
[Route("api/payroll")]
[Tags("Payroll")]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;
    private readonly IMapper _mapper;
    private readonly ILogger<PayrollController> _logger;

    public PayrollController(IPayrollService payrollService, IMapper mapper, ILogger<PayrollController> logger)
    {
        _payrollService = payrollService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Create new payroll record for employee.
    /// TODO: Add [Authorize(Roles = "HR,Admin")] attribute
    /// TODO: Add employee existence validation
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PayrollRecordDto>> CreatePayroll([FromBody] CreatePayrollRecordDto dto)
    {
        // TODO: User to implement:
        // - Validate employee exists
        // - Check duplicate payroll for month
        // - Call service
        // - Log creation event
        // - Return 201 Created

        var domain = _mapper.Map<PayrollRecord>(dto);
        var result = await _payrollService.CreatePayrollAsync(dto.EmployeeId, dto.PayrollMonth, dto.BaseSalary);
        var response = _mapper.Map<PayrollRecordDto>(result);

        return CreatedAtAction(nameof(GetPayrollById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Get payroll record by ID.
    /// TODO: Add authorization (own record, manager, HR)
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PayrollRecordDto>> GetPayrollById(Guid id)
    {
        // TODO: User to implement:
        // - Call service
        // - Check authorization
        // - Return 404 if not found
        // - Add caching

        var payroll = await _payrollService.GetPayrollByIdAsync(id);
        if (payroll == null)
            return NotFound();

        return Ok(_mapper.Map<PayrollRecordDto>(payroll));
    }

    /// <summary>
    /// Get payroll records for employee in date range.
    /// TODO: Add pagination, sorting, date validation
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<List<PayrollRecordDto>>> GetPayrollByEmployee(Guid employeeId, [FromQuery] DateTime? startMonth, [FromQuery] DateTime? endMonth)
    {
        // TODO: User to implement:
        // - Validate date range
        // - Add pagination
        // - Add sorting (date desc)
        // - Check authorization

        startMonth ??= DateTime.UtcNow.AddMonths(-3);
        endMonth ??= DateTime.UtcNow;

        var payrolls = await _payrollService.GetPayrollByEmployeeAsync(employeeId, startMonth.Value, endMonth.Value);
        return Ok(_mapper.Map<List<PayrollRecordDto>>(payrolls));
    }

    /// <summary>
    /// Update payroll record.
    /// TODO: Add status validation (only Pending allowed)
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PayrollRecordDto>> UpdatePayroll(Guid id, [FromBody] UpdatePayrollRecordDto dto)
    {
        // TODO: User to implement:
        // - Check status is "Pending"
        // - Validate updates
        // - Call service
        // - Trigger recalculation

        var updated = await _payrollService.UpdatePayrollAsync(id, payroll =>
        {
            if (dto.PaymentDate.HasValue) payroll.PaymentDate = dto.PaymentDate.Value;
            if (dto.HouseRentAllowance.HasValue) payroll.HouseRentAllowance = dto.HouseRentAllowance.Value;
            if (dto.MedicalAllowance.HasValue) payroll.MedicalAllowance = dto.MedicalAllowance.Value;
            // TODO: User to add remaining field updates
        });

        return Ok(_mapper.Map<PayrollRecordDto>(updated));
    }

    /// <summary>
    /// Process payroll (lock and prepare for payment).
    /// TODO: Add HR/Finance authorization, approval workflow
    /// </summary>
    [HttpPost("{id:guid}/process")]
    public async Task<ActionResult<PayrollRecordDto>> ProcessPayroll(Guid id)
    {
        // TODO: User to implement:
        // - Check authorization (HR/Finance only)
        // - Validate calculations complete
        // - Call service
        // - Trigger notifications
        // - Log audit event

        var processed = await _payrollService.ProcessPayrollAsync(id);
        return Ok(_mapper.Map<PayrollRecordDto>(processed));
    }

    /// <summary>
    /// Mark payroll as paid.
    /// TODO: Add bank reference validation, reconciliation
    /// </summary>
    [HttpPost("{id:guid}/pay")]
    public async Task<ActionResult<PayrollRecordDto>> MarkAsPaid(Guid id, [FromBody] string referenceNumber)
    {
        // TODO: User to implement:
        // - Validate reference number format
        // - Call service
        // - Reconcile with accounting
        // - Send employee notification

        var paid = await _payrollService.MarkAsPaidAsync(id, referenceNumber);
        return Ok(_mapper.Map<PayrollRecordDto>(paid));
    }

    /// <summary>
    /// Get all pending payrolls.
    /// TODO: Add filtering, pagination, HR-only authorization
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<List<PayrollRecordDto>>> GetPendingPayrolls()
    {
        // TODO: User to implement:
        // - Check HR authorization
        // - Add pagination
        // - Add batch processing

        var pending = await _payrollService.GetPendingPayrollsAsync();
        return Ok(_mapper.Map<List<PayrollRecordDto>>(pending));
    }
}
