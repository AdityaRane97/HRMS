using FluentValidation;
using HRMS.Application.DTOs;

namespace HRMS.Application.Validators;

/// <summary>
/// Validator for CreatePayrollRecordDto.
/// TODO: User to implement validation rules for payroll creation
/// Consider: Employee exists, salary ranges, date validity, duplicate prevention, etc.
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
            // TODO: Add rule to prevent past dates or duplicates
            ;

        RuleFor(x => x.PaymentDate)
            .NotEmpty()
            .WithMessage("Payment date is required")
            .GreaterThanOrEqualTo(x => x.PayrollMonth.AddMonths(0))
            .WithMessage("Payment date should be in the same or later month");
            // TODO: Add rule to validate payment date is not before payroll month

        RuleFor(x => x.BaseSalary)
            .GreaterThan(0)
            .WithMessage("Base salary must be greater than zero")
            // TODO: User to add range validation (min/max salary bounds)
            ;

        RuleFor(x => x.HouseRentAllowance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("House rent allowance cannot be negative");
            // TODO: User to add percentage validation (e.g., HRA <= 50% of base)

        RuleFor(x => x.MedicalAllowance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Medical allowance cannot be negative");
            // TODO: User to add ceiling rules

        RuleFor(x => x.TransportAllowance)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Transport allowance cannot be negative");

        RuleFor(x => x.OtherAllowances)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Other allowances cannot be negative");

        // TODO: User to add composite validation:
        // - Total allowances should not exceed X% of base salary
        // - Cross-field dependencies (e.g., if ContractType=Intern, different salary rules)
        // - Database-level checks (employee must exist, not already paid for month, etc.)
    }
}

/// <summary>
/// Validator for UpdatePayrollRecordDto.
/// TODO: User to implement validation rules for payroll updates
/// Consider: Allow updates only if not yet processed, validate new values, etc.
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

        // TODO: User to add:
        // - Validation that payroll is not already processed/paid
        // - Recalculation logic trigger
        // - Approval requirement for salary adjustments
    }
}
