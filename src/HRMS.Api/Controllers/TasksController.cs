using HRMS.Application.DTOs;
using HRMS.Application.Services;
using HRMS.Api.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers;

/// <summary>
/// Task Management API endpoints
/// Supports CRUD operations with role-based access control:
/// - Managers/Admins: Create, assign, edit, and delete tasks
/// - Employees: View assigned tasks and update status
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly CurrentUser _currentUser;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        ITaskService taskService,
        CurrentUser currentUser,
        ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Get tasks for the current user (employees see only assigned tasks, managers/admins see all)
    /// </summary>
    [HttpGet("my-tasks")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<PaginatedTaskResponse>>> GetMyTasks(
        [FromQuery] FilterTasksRequest? filter,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting tasks for current user {UserId}",
            _currentUser.EmployeeId);

        try
        {
            var tasks = await _taskService.GetTasksAsync(
                _currentUser.EmployeeId,
                _currentUser.Role,
                filter,
                cancellationToken);

            return Ok(new ApiResponse<PaginatedTaskResponse>
            {
                Success = true,
                Message = "Tasks retrieved successfully",
                Data = tasks
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks for user {UserId}", _currentUser.EmployeeId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<PaginatedTaskResponse>
                {
                    Success = false,
                    Message = "Failed to retrieve tasks"
                });
        }
    }

    /// <summary>
    /// Get all tasks (admin/manager only)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedTaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PaginatedTaskResponse>>> GetAllTasks(
        [FromQuery] FilterTasksRequest? filter,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting all tasks (admin/manager view)");

        try
        {
            if (!_currentUser.IsManagerOrAdmin())
            {
                return Forbid();
            }

            var tasks = await _taskService.GetAllTasksAsync(filter, cancellationToken);

            return Ok(new ApiResponse<PaginatedTaskResponse>
            {
                Success = true,
                Message = "All tasks retrieved successfully",
                Data = tasks
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tasks");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<PaginatedTaskResponse>
                {
                    Success = false,
                    Message = "Failed to retrieve tasks"
                });
        }
    }

    /// <summary>
    /// Get task details by ID
    /// Employees can only access their assigned tasks
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<TaskDto>>> GetTaskById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting task {TaskId}", id);

        try
        {
            var task = await _taskService.GetTaskByIdAsync(id, cancellationToken);

            if (task == null)
            {
                return NotFound(new ApiResponse<TaskDto>
                {
                    Success = false,
                    Message = "Task not found"
                });
            }

            // Authorization check: employees can only access their assigned tasks
            if (!_currentUser.IsManagerOrAdmin() && task.AssignedEmployeeId != _currentUser.EmployeeId)
            {
                return Forbid();
            }

            return Ok(new ApiResponse<TaskDto>
            {
                Success = true,
                Message = "Task retrieved successfully",
                Data = task
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<TaskDto>
                {
                    Success = false,
                    Message = "Failed to retrieve task"
                });
        }
    }

    /// <summary>
    /// Create a new task (manager/admin only)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<TaskDto>>> CreateTask(
        [FromBody] CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new task for employee {AssignedEmployeeId}", request.AssignedEmployeeId);

        try
        {
            if (!_currentUser.IsManagerOrAdmin())
            {
                return Forbid();
            }

            var task = await _taskService.CreateTaskAsync(
                request,
                _currentUser.EmployeeId,
                cancellationToken);

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id },
                new ApiResponse<TaskDto>
                {
                    Success = true,
                    Message = "Task created successfully",
                    Data = task
                });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for task creation");
            return BadRequest(new ApiResponse<TaskDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<TaskDto>
                {
                    Success = false,
                    Message = "Failed to create task"
                });
        }
    }

    /// <summary>
    /// Update task details (manager/admin only for most fields)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<TaskDto>>> UpdateTask(
        Guid id,
        [FromBody] UpdateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating task {TaskId}", id);

        try
        {
            if (!_currentUser.IsManagerOrAdmin())
            {
                return Forbid();
            }

            request.Id = id;
            var task = await _taskService.UpdateTaskAsync(request, cancellationToken);

            if (task == null)
            {
                return NotFound(new ApiResponse<TaskDto>
                {
                    Success = false,
                    Message = "Task not found"
                });
            }

            return Ok(new ApiResponse<TaskDto>
            {
                Success = true,
                Message = "Task updated successfully",
                Data = task
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for task update");
            return BadRequest(new ApiResponse<TaskDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<TaskDto>
                {
                    Success = false,
                    Message = "Failed to update task"
                });
        }
    }

    /// <summary>
    /// Update task status only (employees can update only assigned task status)
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<TaskDto>>> UpdateTaskStatus(
        Guid id,
        [FromBody] UpdateTaskStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating status for task {TaskId} to {Status}", id, request.Status);

        try
        {
            var task = await _taskService.UpdateTaskStatusAsync(id, request.Status, _currentUser.EmployeeId, cancellationToken);

            if (task == null)
            {
                return NotFound(new ApiResponse<TaskDto>
                {
                    Success = false,
                    Message = "Task not found"
                });
            }

            return Ok(new ApiResponse<TaskDto>
            {
                Success = true,
                Message = "Task status updated successfully",
                Data = task
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for status update");
            return BadRequest(new ApiResponse<TaskDto>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task status for {TaskId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<TaskDto>
                {
                    Success = false,
                    Message = "Failed to update task status"
                });
        }
    }

    /// <summary>
    /// Delete a task (manager/admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTask(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting task {TaskId}", id);

        try
        {
            if (!_currentUser.IsManagerOrAdmin())
            {
                return Forbid();
            }

            var success = await _taskService.DeleteTaskAsync(id, cancellationToken);

            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Task not found"
                });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<object>
                {
                    Success = false,
                    Message = "Failed to delete task"
                });
        }
    }

    /// <summary>
    /// Get task dashboard summary
    /// </summary>
    [HttpGet("dashboard/summary")]
    [ProducesResponseType(typeof(ApiResponse<TaskDashboardSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TaskDashboardSummaryDto>>> GetDashboardSummary(
        [FromQuery] Guid? employeeId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting task dashboard summary");

        try
        {
            // Employees can only get their own summary
            var targetEmployeeId = _currentUser.IsManagerOrAdmin() ? employeeId : _currentUser.EmployeeId;

            var summary = await _taskService.GetTaskDashboardSummaryAsync(targetEmployeeId, cancellationToken);

            return Ok(new ApiResponse<TaskDashboardSummaryDto>
            {
                Success = true,
                Message = "Dashboard summary retrieved successfully",
                Data = summary
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard summary");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<TaskDashboardSummaryDto>
                {
                    Success = false,
                    Message = "Failed to retrieve dashboard summary"
                });
        }
    }

    /// <summary>
    /// Get filter options for task dropdowns
    /// </summary>
    [HttpGet("filter-options")]
    [ProducesResponseType(typeof(ApiResponse<TaskFilterOptionsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<TaskFilterOptionsDto>>> GetFilterOptions(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting task filter options");

        try
        {
            var options = await _taskService.GetFilterOptionsAsync(cancellationToken);

            return Ok(new ApiResponse<TaskFilterOptionsDto>
            {
                Success = true,
                Message = "Filter options retrieved successfully",
                Data = options
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving filter options");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new ApiResponse<TaskFilterOptionsDto>
                {
                    Success = false,
                    Message = "Failed to retrieve filter options"
                });
        }
    }
}
