using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents a task assigned to an employee with tracking for status, priority, and attachments.
/// Supports role-based management: Managers/Admins create and assign tasks,
/// Employees view and update status of assigned tasks.
/// </summary>
public class TaskModel : BaseEntity
{
    /// <summary>
    /// Task title/name
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Detailed task description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Sprint name this task belongs to (e.g., "Sprint 23", "Q4 Initiative")
    /// </summary>
    public string SprintName { get; set; } = string.Empty;

    /// <summary>
    /// Task category for grouping and filtering (e.g., Development, HR, Finance, Sales, Training)
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Task priority level (Low, Medium, High)
    /// </summary>
    public string Priority { get; set; } = "Medium"; // Low, Medium, High

    /// <summary>
    /// Current status of the task (To Do, In Progress, On Hold, Completed)
    /// </summary>
    public string Status { get; set; } = "To Do"; // To Do, In Progress, On Hold, Completed

    /// <summary>
    /// Task start date
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Task due date/deadline
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Employee ID of the person assigned to this task
    /// </summary>
    public Guid AssignedEmployeeId { get; set; }

    /// <summary>
    /// Navigation property to the assigned Employee
    /// </summary>
    public virtual Employee? AssignedEmployee { get; set; }

    /// <summary>
    /// Employee ID of the person who created/assigned this task (manager/admin)
    /// </summary>
    public Guid CreatedByEmployeeId { get; set; }

    /// <summary>
    /// Navigation property to the creating Employee
    /// </summary>
    public virtual Employee? CreatedByEmployee { get; set; }

    /// <summary>
    /// Optional notes or additional context about the task
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Date when task was marked as completed
    /// </summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>
    /// Collection of documents/attachments associated with this task
    /// </summary>
    public virtual ICollection<TaskDocument> TaskDocuments { get; set; } = new List<TaskDocument>();

    /// <summary>
    /// Soft delete flag - task can be marked as deleted without removing from database
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// Validation: Ensure due date is not before start date
    /// </summary>
    public bool IsValid() => DueDate >= StartDate;
}
