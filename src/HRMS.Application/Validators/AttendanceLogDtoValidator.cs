using FluentValidation;
using HRMS.Application.DTOs;

namespace HRMS.Application.Validators;

/// <summary>
/// Validator for AttendanceCheckInDto.
/// TODO: User to implement validation for check-in
/// Consider: Employee active, not already checked in, time zone handling, geofencing, etc.
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
            // TODO: Add validation to prevent duplicate check-ins for same day

        RuleFor(x => x.Location)
            .NotEmpty()
            .WithMessage("Location is required");
            // TODO: Add location validation (allowed locations, geofencing, etc.)
    }
}

/// <summary>
/// Validator for AttendanceCheckOutDto.
/// TODO: User to implement validation for check-out
/// Consider: Employee checked in, checkout after checkin, minimum work duration, etc.
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
            // TODO: Add validation that check-out is after corresponding check-in
            // TODO: Add validation for minimum work duration (e.g., at least 4 hours)

        RuleFor(x => x.Remarks)
            .MaximumLength(500)
            .WithMessage("Remarks cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Remarks));
    }
}

/// <summary>
/// Validator for AttendanceApprovalDto (HR/Manager override).
/// TODO: User to implement validation for attendance adjustments
/// Consider: Valid status values, only by authorized users, not for future dates, etc.
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

        // TODO: User to add:
        // - Validation that approver has authority (HR/Manager)
        // - Concurrent approval prevention (same date/employee)
        // - Audit trail requirement
    }
}
