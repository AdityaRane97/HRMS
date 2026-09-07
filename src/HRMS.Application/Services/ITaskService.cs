using HRMS.Application.DTOs;

namespace HRMS.Application.Services;

/// <summary>
/// Service interface for Task management operations
/// Handles all business logic for task CRUD, filtering, and authorization
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Get tasks for a user (filtered based on role/permissions)
    /// </summary>
    Task<PaginatedTaskResponse> GetTasksAsync(
        Guid userId,
        string userRole,
        FilterTasksRequest? filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all tasks in the system (admin/manager only)
    /// </summary>
    Task<PaginatedTaskResponse> GetAllTasksAsync(
        FilterTasksRequest? filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a specific task by ID with full details
    /// </summary>
    Task<TaskDto?> GetTaskByIdAsync(
        Guid taskId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new task
    /// </summary>
    Task<TaskDto> CreateTaskAsync(
        CreateTaskRequest request,
        Guid createdByUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing task
    /// </summary>
    Task<TaskDto?> UpdateTaskAsync(
        UpdateTaskRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update task status (can be used by employees for assigned tasks)
    /// </summary>
    Task<TaskDto?> UpdateTaskStatusAsync(
        Guid taskId,
        string newStatus,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a task
    /// </summary>
    Task<bool> DeleteTaskAsync(
        Guid taskId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get dashboard summary with task counts and recent tasks
    /// </summary>
    Task<TaskDashboardSummaryDto> GetTaskDashboardSummaryAsync(
        Guid? employeeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get filter options (statuses, priorities, categories, etc.)
    /// </summary>
    Task<TaskFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user can access a specific task
    /// </summary>
    Task<bool> CanUserAccessTaskAsync(
        Guid taskId,
        Guid userId,
        string userRole,
        CancellationToken cancellationToken = default);
}
