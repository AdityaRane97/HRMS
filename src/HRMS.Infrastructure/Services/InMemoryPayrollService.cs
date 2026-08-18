using HRMS.Application.Services;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Services;

/// <summary>
/// In-memory payroll service for Phase 2 (placeholder; replace with database-backed in Phase 3).
/// Handles payroll record creation, calculation, and payment processing.
/// TODO: Implement persistent storage in Phase 3
/// </summary>
public class InMemoryPayrollService : IPayrollService
{
    private readonly Dictionary<Guid, PayrollRecord> _payrollRecords = new();

    /// <summary>
    /// Create a new payroll record for an employee.
    /// Initializes with base salary, calculates gross/net, and stores.
    /// </summary>
    public async Task<PayrollRecord> CreatePayrollAsync(Guid employeeId, DateTime payrollMonth, decimal baseSalary)
    {
        var id = Guid.NewGuid();
        var payroll = new PayrollRecord(employeeId, payrollMonth, baseSalary)
        {
            Id = id,
            CreatedAt = DateTime.UtcNow
        };

        // Calculate salary components upon creation
        payroll.CalculateGrossSalary();
        payroll.CalculateDeductions();
        payroll.CalculateNetSalary();

        _payrollRecords[id] = payroll;
        return await Task.FromResult(payroll);
    }

    /// <summary>
    /// Get payroll record by ID.
    /// Throws if not found.
    /// </summary>
    public async Task<PayrollRecord?> GetPayrollByIdAsync(Guid payrollId)
    {
        _payrollRecords.TryGetValue(payrollId, out var payroll);
        return await Task.FromResult(payroll);
    }

    /// <summary>
    /// Get all payroll records for an employee in a date range.
    /// Filters by employee ID and payroll month range.
    /// </summary>
    public async Task<List<PayrollRecord>> GetPayrollByEmployeeAsync(Guid employeeId, DateTime startMonth, DateTime endMonth)
    {
        var records = _payrollRecords.Values
            .Where(p => p.EmployeeId == employeeId 
                && p.PayrollMonth >= startMonth 
                && p.PayrollMonth <= endMonth)
            .OrderBy(p => p.PayrollMonth)
            .ToList();

        return await Task.FromResult(records);
    }

    /// <summary>
    /// Update payroll record (before processing).
    /// Validates status is Pending, updates fields, and recalculates.
    /// </summary>
    public async Task<PayrollRecord> UpdatePayrollAsync(Guid payrollId, Action<PayrollRecord> updateAction)
    {
        if (!_payrollRecords.TryGetValue(payrollId, out var payroll))
            throw new KeyNotFoundException($"Payroll record {payrollId} not found");

        if (payroll.PaymentStatus != "Pending")
            throw new InvalidOperationException("Only pending payroll can be updated");

        payroll.UpdatedAt = DateTime.UtcNow;
        updateAction(payroll);

        // Recalculate after updates
        payroll.CalculateGrossSalary();
        payroll.CalculateDeductions();
        payroll.CalculateNetSalary();

        return await Task.FromResult(payroll);
    }

    /// <summary>
    /// Process payroll (calculate final amounts, lock for payment).
    /// Calls domain method to transition status.
    /// </summary>
    public async Task<PayrollRecord> ProcessPayrollAsync(Guid payrollId)
    {
        if (!_payrollRecords.TryGetValue(payrollId, out var payroll))
            throw new KeyNotFoundException($"Payroll record {payrollId} not found");

        payroll.MarkAsProcessed();
        payroll.UpdatedAt = DateTime.UtcNow;

        return await Task.FromResult(payroll);
    }

    /// <summary>
    /// Mark payroll as paid after bank transfer.
    /// Records payment reference and marks as paid.
    /// </summary>
    public async Task<PayrollRecord> MarkAsPaidAsync(Guid payrollId, string referenceNumber)
    {
        if (!_payrollRecords.TryGetValue(payrollId, out var payroll))
            throw new KeyNotFoundException($"Payroll record {payrollId} not found");

        payroll.MarkAsPaid();
        payroll.ReferenceNumber = referenceNumber;
        payroll.UpdatedAt = DateTime.UtcNow;

        return await Task.FromResult(payroll);
    }

    /// <summary>
    /// Get pending payroll records for processing.
    /// Returns all payroll in "Pending" status, sorted by month.
    /// </summary>
    public async Task<List<PayrollRecord>> GetPendingPayrollsAsync()
    {
        var pending = _payrollRecords.Values
            .Where(p => p.PaymentStatus == "Pending")
            .OrderBy(p => p.PayrollMonth)
            .ToList();

        return await Task.FromResult(pending);
    }
}
