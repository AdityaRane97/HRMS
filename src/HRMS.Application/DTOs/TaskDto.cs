namespace HRMS.Application.DTOs;

/// <summary>
/// Data Transfer Object for Task entity in API responses
/// </summary>
public class TaskDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string SprintName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    public Guid AssignedEmployeeId { get; set; }

    public string AssignedEmployeeName { get; set; } = string.Empty;

    public string AssignedEmployeeEmail { get; set; } = string.Empty;

    public Guid CreatedByEmployeeId { get; set; }

    public string CreatedByEmployeeName { get; set; } = string.Empty;

    public string CreatedByEmail { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CompletedDate { get; set; }

    public List<TaskDocumentDto> AttachedDocuments { get; set; } = new();
}

/// <summary>
/// Document attached to a task
/// </summary>
public class TaskDocumentDto
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FileType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadDate { get; set; }

    public string? DocumentUrl { get; set; }

    public string? DocumentPurpose { get; set; }

    public bool IsPrimary { get; set; }

    public string UploadedByEmail { get; set; } = string.Empty;
}

/// <summary>
/// Minimal task response for list views
/// </summary>
public class TaskListItemDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string AssignedEmployeeName { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public DateTime DueDate { get; set; }

    public string Category { get; set; } = string.Empty;

    public string SprintName { get; set; } = string.Empty;
}

/// <summary>
/// Request for creating a new task
/// </summary>
public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string SprintName { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Priority { get; set; } = "Medium";

    public string Status { get; set; } = "To Do";

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    public Guid AssignedEmployeeId { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Request for updating an existing task
/// </summary>
public class UpdateTaskRequest
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? SprintName { get; set; }

    public string? Category { get; set; }

    public string? Priority { get; set; }

    public string? Status { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid? AssignedEmployeeId { get; set; }

    public string? Notes { get; set; }
}

/// <summary>
/// Request for updating only task status
/// </summary>
public class UpdateTaskStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Filter criteria for querying tasks
/// </summary>
public class FilterTasksRequest
{
    public string? SearchTerm { get; set; }

    public List<string>? Statuses { get; set; }

    public List<string>? Priorities { get; set; }

    public List<Guid>? AssignedEmployeeIds { get; set; }

    public List<string>? Categories { get; set; }

    public List<string>? Sprints { get; set; }

    public DateTime? DateFrom { get; set; }

    public DateTime? DateTo { get; set; }

    public string? SortBy { get; set; } = "DueDate";

    public string? SortOrder { get; set; } = "asc";

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}

/// <summary>
/// Response for paginated task listing
/// </summary>
public class PaginatedTaskResponse
{
    public List<TaskListItemDto> Items { get; set; } = new();

    public int TotalCount { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }
}

/// <summary>
/// Task dashboard summary
/// </summary>
public class TaskDashboardSummaryDto
{
    public int TotalTasks { get; set; }

    public int CompletedTasks { get; set; }

    public int OverdueTasks { get; set; }

    public Dictionary<string, int> StatusSummary { get; set; } = new();

    public List<TaskListItemDto> RecentTasks { get; set; } = new();
}

/// <summary>
/// Filter options for task dropdowns
/// </summary>
public class TaskFilterOptionsDto
{
    public List<string> Statuses { get; set; } = new();

    public List<string> Priorities { get; set; } = new();

    public List<string> Categories { get; set; } = new();

    public List<string> Sprints { get; set; } = new();

    public List<EmployeeOptionDto> Employees { get; set; } = new();
}

/// <summary>
/// Employee option for dropdown/search
/// </summary>
public class EmployeeOptionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}
