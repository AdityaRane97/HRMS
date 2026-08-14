using FluentValidation;
using HRMS.Application.DTOs;

namespace HRMS.Application.Validators;

/// <summary>
/// Validators for Employee DTOs.
/// Ensures data integrity before reaching business logic.
/// </summary>
public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be a valid email address")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters");

        RuleFor(x => x.EmployeeCode)
            .NotEmpty().WithMessage("Employee code is required")
            .MaximumLength(50).WithMessage("Employee code must not exceed 50 characters");

        RuleFor(x => x.JoinDate)
            .NotEmpty().WithMessage("Join date is required")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Join date cannot be in the future");

        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Organization ID is required");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
            .Matches(@"^\+?[1-9]\d{1,14}$|^\s*$").WithMessage("Phone number must be valid")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Department)
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters");

        RuleFor(x => x.Designation)
            .MaximumLength(100).WithMessage("Designation must not exceed 100 characters");
    }
}

public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.LastName));

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

        RuleFor(x => x.Department)
            .MaximumLength(100).WithMessage("Department must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Department));

        RuleFor(x => x.Designation)
            .MaximumLength(100).WithMessage("Designation must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Designation));

        RuleFor(x => x.EmploymentStatus)
            .Must(x => new[] { "Active", "OnLeave", "Resigned", "Retired" }.Contains(x))
            .WithMessage("Employment status must be one of: Active, OnLeave, Resigned, Retired")
            .When(x => !string.IsNullOrEmpty(x.EmploymentStatus));
    }
}
