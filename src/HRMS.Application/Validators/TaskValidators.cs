using FluentValidation;
using HRMS.Application.DTOs;

namespace HRMS.Application.Validators;

/// <summary>
/// Validator for CreateTaskRequest
/// Ensures all required fields are present and valid before task creation
/// </summary>
public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MinimumLength(5).WithMessage("Task title must be at least 5 characters")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Task description is required")
            .MinimumLength(10).WithMessage("Task description must be at least 10 characters")
            .MaximumLength(2000).WithMessage("Task description cannot exceed 2000 characters");

        RuleFor(x => x.SprintName)
            .NotEmpty().WithMessage("Sprint name is required")
            .MaximumLength(100).WithMessage("Sprint name cannot exceed 100 characters");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required")
            .MaximumLength(50).WithMessage("Category cannot exceed 50 characters");

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required")
            .Must(p => new[] { "Low", "Medium", "High" }.Contains(p))
            .WithMessage("Priority must be Low, Medium, or High");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(s => new[] { "To Do", "In Progress", "On Hold", "Completed" }.Contains(s))
            .WithMessage("Status must be To Do, In Progress, On Hold, or Completed");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .LessThanOrEqualTo(x => x.DueDate).WithMessage("Start date must be before or equal to due date");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Due date must be after or equal to start date");

        RuleFor(x => x.AssignedEmployeeId)
            .NotEmpty().WithMessage("Assigned employee is required");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

/// <summary>
/// Validator for UpdateTaskRequest
/// Only validates fields that are being updated
/// </summary>
public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Task ID is required");

        RuleFor(x => x.Title)
            .MinimumLength(5).WithMessage("Task title must be at least 5 characters")
            .MaximumLength(200).WithMessage("Task title cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Title));

        RuleFor(x => x.Description)
            .MinimumLength(10).WithMessage("Task description must be at least 10 characters")
            .MaximumLength(2000).WithMessage("Task description cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Priority)
            .Must(p => new[] { "Low", "Medium", "High" }.Contains(p))
            .WithMessage("Priority must be Low, Medium, or High")
            .When(x => !string.IsNullOrEmpty(x.Priority));

        RuleFor(x => x.Status)
            .Must(s => new[] { "To Do", "In Progress", "On Hold", "Completed" }.Contains(s))
            .WithMessage("Status must be To Do, In Progress, On Hold, or Completed")
            .When(x => !string.IsNullOrEmpty(x.Status));

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
}

/// <summary>
/// Validator for UpdateTaskStatusRequest
/// </summary>
public class UpdateTaskStatusRequestValidator : AbstractValidator<UpdateTaskStatusRequest>
{
    public UpdateTaskStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required")
            .Must(s => new[] { "To Do", "In Progress", "On Hold", "Completed" }.Contains(s))
            .WithMessage("Status must be To Do, In Progress, On Hold, or Completed");
    }
}
