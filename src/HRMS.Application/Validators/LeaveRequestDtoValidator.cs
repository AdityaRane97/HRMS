using FluentValidation;
using HRMS.Application.DTOs;

namespace HRMS.Application.Validators;

/// <summary>
/// Validator for CreateLeaveRequestDto.
/// Validates leave type, date range, reason, and half-day configuration.
/// </summary>
public class CreateLeaveRequestDtoValidator : AbstractValidator<CreateLeaveRequestDto>
{
    private static readonly string[] ValidLeaveTypes = 
    { 
        "Annual", "Sick", "PersonalCare", "Maternity", "Paternity", "Sabbatical", "Unpaid" 
    };

    public CreateLeaveRequestDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("Employee ID is required");

        RuleFor(x => x.LeaveType)
            .NotEmpty()
            .WithMessage("Leave type is required")
            .Must(type => ValidLeaveTypes.Contains(type))
            .WithMessage($"Leave type must be one of: {string.Join(", ", ValidLeaveTypes)}");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after start date");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MinimumLength(10)
            .WithMessage("Reason must be at least 10 characters")
            .MaximumLength(1000)
            .WithMessage("Reason cannot exceed 1000 characters");

        RuleFor(x => x.ReplacementEmployeeId)
            .MaximumLength(100)
            .WithMessage("Replacement employee ID cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.ReplacementEmployeeId));

        RuleFor(x => x.HalfDayPeriod)
            .Must(period => period == null || period == "FirstHalf" || period == "SecondHalf")
            .WithMessage("Half-day period must be 'FirstHalf' or 'SecondHalf'")
            .When(x => x.IsHalfDay);

        RuleFor(x => x.IsHalfDay)
            .Must((dto, isHalfDay) => !(isHalfDay && string.IsNullOrEmpty(dto.HalfDayPeriod)))
            .WithMessage("Half-day period is required when IsHalfDay is true");
    }
}

/// <summary>
/// Validator for ApproveLeaveByManagerDto.
/// Validates manager ID and approval remarks.
/// </summary>
public class ApproveLeaveByManagerDtoValidator : AbstractValidator<ApproveLeaveByManagerDto>
{
    public ApproveLeaveByManagerDtoValidator()
    {
        RuleFor(x => x.ManagerId)
            .NotEmpty()
            .WithMessage("Manager ID is required");

        RuleFor(x => x.Remarks)
            .MaximumLength(500)
            .WithMessage("Remarks cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Remarks));
    }
}

/// <summary>
/// Validator for ApproveLeaveByHRDto.
/// Validates HR approver ID and approval remarks.
/// </summary>
public class ApproveLeaveByHRDtoValidator : AbstractValidator<ApproveLeaveByHRDto>
{
    public ApproveLeaveByHRDtoValidator()
    {
        RuleFor(x => x.HRApproverId)
            .NotEmpty()
            .WithMessage("HR approver ID is required");

        RuleFor(x => x.Remarks)
            .MaximumLength(500)
            .WithMessage("Remarks cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Remarks));
    }
}

/// <summary>
/// Validator for CancelLeaveDto.
/// Validates cancellation reason requirement.
/// </summary>
public class CancelLeaveDtoValidator : AbstractValidator<CancelLeaveDto>
{
    public CancelLeaveDtoValidator()
    {
        RuleFor(x => x.CancellationReason)
            .NotEmpty()
            .WithMessage("Cancellation reason is required")
            .MinimumLength(10)
            .WithMessage("Reason must be at least 10 characters")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}
