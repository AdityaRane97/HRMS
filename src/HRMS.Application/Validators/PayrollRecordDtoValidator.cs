using FluentValidation;
using HRMS.Application.DTOs;

namespace HRMS.Application.Validators;

/// <summary>
/// Validator for CreatePayrollRecordDto.
/// Validates employee ID, salary components, and date ranges.
/// </summary>
public class CreatePayrollRecordDtoValidator : AbstractValidator<CreatePayrollRecordDto>
{
    public CreatePayrollRecordDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("Employee ID is required");

        RuleFor(x => x.PayrollMonth)
            .NotEmpty()
            .WithMessage("Payroll month is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Payroll month cannot be in the future");

        RuleFor(x => x.PaymentDate)
            .NotEmpty()
            .WithMessage("Payment date is required")
            .GreaterThanOrEqualTo(x => x.PayrollMonth)
            .WithMessage("Payment date must be in or after payroll month");

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0)
            .WithMessage("Base salary must be greater than zero")
            .LessThanOrEqualTo(10000000)
            .WithMessage("Base salary cannot exceed company maximum");

        RuleFor(x => x.HouseRentAllowance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("House rent allowance cannot be negative");

        RuleFor(x => x.MedicalAllowance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Medical allowance cannot be negative");

        RuleFor(x => x.TransportAllowance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Transport allowance cannot be negative");

        RuleFor(x => x.OtherAllowances)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Other allowances cannot be negative");
    }
}

/// <summary>
/// Validator for UpdatePayrollRecordDto.
/// Validates salary component updates only for pending payroll.
/// </summary>
public class UpdatePayrollRecordDtoValidator : AbstractValidator<UpdatePayrollRecordDto>
{
    public UpdatePayrollRecordDtoValidator()
    {
        RuleFor(x => x.PaymentDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Payment date cannot be in the past")
            .When(x => x.PaymentDate.HasValue);

        RuleFor(x => x.HouseRentAllowance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("House rent allowance cannot be negative")
            .When(x => x.HouseRentAllowance.HasValue);

        RuleFor(x => x.MedicalAllowance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Medical allowance cannot be negative")
            .When(x => x.MedicalAllowance.HasValue);

        RuleFor(x => x.TransportAllowance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Transport allowance cannot be negative")
            .When(x => x.TransportAllowance.HasValue);

        RuleFor(x => x.OtherAllowances)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Other allowances cannot be negative")
            .When(x => x.OtherAllowances.HasValue);

        RuleFor(x => x.IncomeTax)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Income tax cannot be negative")
            .When(x => x.IncomeTax.HasValue);

        RuleFor(x => x.ProvidentFund)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Provident fund cannot be negative")
            .When(x => x.ProvidentFund.HasValue);

        RuleFor(x => x.HealthInsurance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Health insurance cannot be negative")
            .When(x => x.HealthInsurance.HasValue);

        RuleFor(x => x.OtherDeductions)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Other deductions cannot be negative")
            .When(x => x.OtherDeductions.HasValue);
    }
}
