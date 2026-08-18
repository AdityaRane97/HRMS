using FluentValidation;
using HRMS.Application.DTOs;

namespace HRMS.Application.Validators;

/// <summary>
/// Validator for CreateLeaveRequestDto.
/// TODO: User to implement validation for leave request creation
/// Consider: Employee active, date validity, leave balance, overlapping leaves, etc.
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

        // TODO: User to add:
        // - Employee must exist and be active
        // - Check leave balance for the leave type
        // - Check for overlapping leave requests
        // - Advance notice requirement (e.g., 5 days for annual leave)
        // - Blackout dates/periods
        // - Manager must be assigned to employee
        // - Replacement employee validation (if provided)
    }
}

/// <summary>
/// Validator for ApproveLeaveByManagerDto.
/// TODO: User to implement validation for manager approval
/// Consider: Manager is actually the employee's manager, leave is still pending, etc.
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

        // TODO: User to add:
        // - Validation that remarker is truly the manager of the employee
        // - Rejection reason is required if !IsApproved
        // - Cannot approve leaves in the past
        // - Cannot re-approve already processed leaves
    }
}

/// <summary>
/// Validator for ApproveLeaveByHRDto (HR final approval).
/// TODO: User to implement validation for HR approval
/// Consider: HR authority, manager approval already obtained, etc.
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

        // TODO: User to add:
        // - Validation that approver is HR role
        // - Leave must have been already approved by manager
        // - Rejection reason is required if !IsApproved
        // - Cannot approve leaves not yet starting
        // - Audit log requirement
        // - Notification requirement to employee
    }
}

/// <summary>
/// Validator for CancelLeaveDto.
/// TODO: User to implement validation for leave cancellation
/// Consider: Leave is approved, not yet started, authorization, etc.
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

        // TODO: User to add:
        // - Cannot cancel leaves that have already started
        // - Cannot cancel leaves that are already cancelled
        // - Cancellation requires notification to manager/HR
        // - Leave balance restoration logic
        // - Audit trail
    }
}
