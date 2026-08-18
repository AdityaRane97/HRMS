namespace HRMS.Application.DTOs;

/// <summary>
/// DTO for PayrollRecord response (read-only API response).
/// </summary>
public class PayrollRecordDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeFullName { get; set; } = string.Empty;
    public DateTime PayrollMonth { get; set; }
    public DateTime PaymentDate { get; set; }

    // Salary Components
    public decimal BaseSalary { get; set; }
    public decimal HouseRentAllowance { get; set; }
    public decimal MedicalAllowance { get; set; }
    public decimal TransportAllowance { get; set; }
    public decimal OtherAllowances { get; set; }

    // Deductions
    public decimal IncomeTax { get; set; }
    public decimal ProvidentFund { get; set; }
    public decimal HealthInsurance { get; set; }
    public decimal OtherDeductions { get; set; }

    // Calculated
    public decimal GrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }

    // Status
    public string PaymentStatus { get; set; } = "Pending";
    public DateTime? ProcessedAt { get; set; }
    public DateTime? PaidAt { get; set; }

    public string? Remarks { get; set; }
    public string? ReferenceNumber { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a payroll record.
/// TODO: User to review - may need adjustment for your payroll workflow
/// </summary>
public class CreatePayrollRecordDto
{
    public Guid EmployeeId { get; set; }
    public DateTime PayrollMonth { get; set; }
    public DateTime PaymentDate { get; set; }

    public decimal BaseSalary { get; set; }
    public decimal HouseRentAllowance { get; set; } = 0;
    public decimal MedicalAllowance { get; set; } = 0;
    public decimal TransportAllowance { get; set; } = 0;
    public decimal OtherAllowances { get; set; } = 0;

    public decimal IncomeTax { get; set; } = 0;
    public decimal ProvidentFund { get; set; } = 0;
    public decimal HealthInsurance { get; set; } = 0;
    public decimal OtherDeductions { get; set; } = 0;

    public string? Remarks { get; set; }
}

/// <summary>
/// DTO for updating a payroll record (before processing/payment).
/// TODO: User to consider if salary adjustments should be allowed (new rules, retroactive changes, etc.)
/// </summary>
public class UpdatePayrollRecordDto
{
    public DateTime? PaymentDate { get; set; }

    public decimal? HouseRentAllowance { get; set; }
    public decimal? MedicalAllowance { get; set; }
    public decimal? TransportAllowance { get; set; }
    public decimal? OtherAllowances { get; set; }

    public decimal? IncomeTax { get; set; }
    public decimal? ProvidentFund { get; set; }
    public decimal? HealthInsurance { get; set; }
    public decimal? OtherDeductions { get; set; }

    public string? Remarks { get; set; }
}
