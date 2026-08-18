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
    /// Calculate gross salary from base + allowances.
    /// Deduction strategy deferred to Employee configuration (Phase 2.1).
    /// </summary>
    public void CalculateGrossSalary()
    {
        GrossSalary = BaseSalary 
            + HouseRentAllowance 
            + MedicalAllowance 
            + TransportAllowance 
            + OtherAllowances;
    }

    /// <summary>
    /// Calculate total deductions.
    /// Phase 2.1: Deductions stored as-is (from pre-configuration or UI input).
    /// Future: Implement dynamic deduction rules from Employee config.
    /// </summary>
    public void CalculateDeductions()
    {
        TotalDeductions = IncomeTax + ProvidentFund + HealthInsurance + OtherDeductions;
    }

    /// <summary>
    /// Calculate net salary (GrossSalary - TotalDeductions).
    /// Call after CalculateGrossSalary() and CalculateDeductions().
    /// </summary>
    public void CalculateNetSalary()
    {
        NetSalary = GrossSalary - TotalDeductions;
        // Ensure net is not negative (fail-safe)
        if (NetSalary < 0)
            NetSalary = 0;
    }

    /// <summary>
    /// Mark payroll as processed (ready for payment).
    /// Sets PaymentStatus to "Processed" and records timestamp.
    /// </summary>
    public void MarkAsProcessed()
    {
        if (PaymentStatus != "Pending")
            throw new InvalidOperationException("Only pending payroll can be marked as processed");

        PaymentStatus = "Processed";
        ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Mark payroll as paid.
    /// Sets PaymentStatus to "Paid" and records payment timestamp.
    /// </summary>
    public void MarkAsPaid()
    {
        if (PaymentStatus != "Processed")
            throw new InvalidOperationException("Only processed payroll can be marked as paid");

        PaymentStatus = "Paid";
        PaidAt = DateTime.UtcNow;
    }
}
