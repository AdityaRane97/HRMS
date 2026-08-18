using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents a payroll record for an employee.
/// Tracks salary components, deductions, and payment details.
/// </summary>
public class PayrollRecord : BaseEntity
{
    // Employee Reference
    public Guid EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    // Payroll Period
    public DateTime PayrollMonth { get; set; }
    public DateTime PaymentDate { get; set; }

    // Salary Components
    public decimal BaseSalary { get; set; }
    public decimal HouseRentAllowance { get; set; } = 0;
    public decimal MedicalAllowance { get; set; } = 0;
    public decimal TransportAllowance { get; set; } = 0;
    public decimal OtherAllowances { get; set; } = 0;

    // Deductions
    public decimal IncomeTax { get; set; } = 0;
    public decimal ProvidentFund { get; set; } = 0; // PF/401k equivalent
    public decimal HealthInsurance { get; set; } = 0;
    public decimal OtherDeductions { get; set; } = 0;

    // Calculated Fields
    public decimal GrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }

    // Status
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Processed, Paid, Failed
    public DateTime? ProcessedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    // Additional Info
    public string? Remarks { get; set; }
    public string? ReferenceNumber { get; set; } // Bank transfer reference

    public PayrollRecord()
    {
    }

    public PayrollRecord(Guid employeeId, DateTime payrollMonth, decimal baseSalary)
    {
        EmployeeId = employeeId;
        PayrollMonth = payrollMonth;
        BaseSalary = baseSalary;
    }

    /// <summary>
    /// Calculate gross salary from allowances.
    /// TODO: Implement allowance calculation logic
    /// Consider: Additional allowances, bonus, incentives, performance bonuses, etc.
    /// </summary>
    public void CalculateGrossSalary()
    {
        // TODO: User to implement gross salary calculation
        // Example logic seed:
        // GrossSalary = BaseSalary + HouseRentAllowance + MedicalAllowance + TransportAllowance + OtherAllowances;

        throw new NotImplementedException("CalculateGrossSalary must be implemented with your allowance rules");
    }

    /// <summary>
    /// Calculate total deductions.
    /// TODO: Implement deduction calculation logic
    /// Consider: Tax brackets, progressive taxation, statutory deductions, optional deductions, etc.
    /// </summary>
    public void CalculateDeductions()
    {
        // TODO: User to implement deduction calculation
        // Example logic seed:
        // TotalDeductions = IncomeTax + ProvidentFund + HealthInsurance + OtherDeductions;

        throw new NotImplementedException("CalculateDeductions must be implemented with your deduction rules");
    }

    /// <summary>
    /// Calculate net salary (GrossSalary - TotalDeductions).
    /// TODO: Call this after CalculateGrossSalary and CalculateDeductions
    /// </summary>
    public void CalculateNetSalary()
    {
        // TODO: User to implement net salary calculation
        // Example logic seed:
        // NetSalary = GrossSalary - TotalDeductions;
        // Ensure NetSalary is never negative

        throw new NotImplementedException("CalculateNetSalary must be implemented");
    }

    /// <summary>
    /// Mark payroll as processed.
    /// TODO: Add validation (all calculations done, approvals obtained, etc.)
    /// </summary>
    public void MarkAsProcessed()
    {
        // TODO: User to implement processing logic
        // Consider: Validation, audit log, notification, etc.

        throw new NotImplementedException("MarkAsProcessed must be implemented");
    }

    /// <summary>
    /// Mark payroll as paid.
    /// TODO: Add payment confirmation logic
    /// </summary>
    public void MarkAsPaid()
    {
        // TODO: User to implement payment confirmation logic
        // Consider: Update PaidAt, verify bank transfer, etc.

        throw new NotImplementedException("MarkAsPaid must be implemented");
    }
}
