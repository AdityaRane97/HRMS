using FluentValidation;
using HRMS.Application.DTOs;

namespace HRMS.Application.Validators;

/// <summary>
/// Validator for AttendanceCheckInDto.
/// Validates employee ID, check-in time, and location requirements.
/// </summary>
public class AttendanceCheckInDtoValidator : AbstractValidator<AttendanceCheckInDto>
{
    public AttendanceCheckInDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("Employee ID is required");

        RuleFor(x => x.CheckInTime)
            .NotEmpty()
            .WithMessage("Check-in time is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Check-in time cannot be in the future");

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required")
            .MaximumLength(100)
            .WithMessage("Location cannot exceed 100 characters");
    }
}

/// <summary>
/// Validator for AttendanceCheckOutDto.
/// Validates employee ID, check-out time, and remarks.
/// </summary>
public class AttendanceCheckOutDtoValidator : AbstractValidator<AttendanceCheckOutDto>
{
    public AttendanceCheckOutDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("Employee ID is required");

        RuleFor(x => x.CheckOutTime)
            .NotEmpty()
            .WithMessage("Check-out time is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Check-out time cannot be in the future");

        RuleFor(x => x.Remarks)
            .MaximumLength(500)
            .WithMessage("Remarks cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Remarks));
    }
}

/// <summary>
/// Validator for AttendanceApprovalDto.
/// Validates attendance status override, worked hours, and approval remarks.
/// </summary>
public class AttendanceApprovalDtoValidator : AbstractValidator<AttendanceApprovalDto>
{
    private static readonly string[] ValidStatuses = 
    { 
        "Present", "Absent", "LeaveApproved", "HalfDay", "LateArrival", "EarlyLeave", "WFH" 
    };

    public AttendanceApprovalDtoValidator()
    {
        RuleFor(x => x.EmployeeId)
            .NotEmpty()
            .WithMessage("Employee ID is required");

        RuleFor(x => x.AttendanceDate)
            .NotEmpty()
            .WithMessage("Attendance date is required")
            .LessThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Cannot approve future attendance");

        RuleFor(x => x.AttendanceStatus)
            .NotEmpty()
            .WithMessage("Attendance status is required")
            .Must(status => ValidStatuses.Contains(status))
            .WithMessage($"Attendance status must be one of: {string.Join(", ", ValidStatuses)}");

        RuleFor(x => x.WorkedHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Worked hours cannot be negative")
            .LessThanOrEqualTo(24)
            .WithMessage("Worked hours cannot exceed 24")
            .When(x => x.WorkedHours.HasValue);

        RuleFor(x => x.ApprovalRemarks)
            .MaximumLength(500)
            .WithMessage("Approval remarks cannot exceed 500 characters");
    }
}
