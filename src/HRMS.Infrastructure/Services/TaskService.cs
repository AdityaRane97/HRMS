using AutoMapper;
using HRMS.Application.DTOs;
using HRMS.Application.Services;
using HRMS.Domain.Entities;
using HRMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HRMS.Infrastructure.Services;

/// <summary>
/// Implementation of ITaskService
/// Handles all task-related business logic with proper authorization and validation
/// </summary>
public class TaskService : ITaskService
{
    private readonly HrmsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TaskService> _logger;

    private const string RoleAdmin = "ADMIN";
    private const string RoleManager = "MANAGER";
    private const string RoleEmployee = "EMPLOYEE";

    public TaskService(
        HrmsDbContext context,
        IMapper mapper,
        ILogger<TaskService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    async Task<PaginatedTaskResponse> ITaskService.GetTasksAsync(
        Guid userId,
        string userRole,
        FilterTasksRequest? filter,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting tasks for user {UserId} with role {Role}", userId, userRole);

        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => !t.IsDeleted);

        // Employees only see their assigned tasks
        if (userRole == RoleEmployee)
        {
            query = query.Where(t => t.AssignedEmployeeId == userId);
        }

        // Apply filters
        query = ApplyFilters(query, filter);

        return await GetPaginatedResponseAsync(query, filter, cancellationToken);
    }

    async System.Threading.Tasks.Task<PaginatedTaskResponse> ITaskService.GetAllTasksAsync(
        FilterTasksRequest? filter,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all tasks");

        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => !t.IsDeleted);

        query = ApplyFilters(query, filter);

        return await GetPaginatedResponseAsync(query, filter, cancellationToken);
    }

    async System.Threading.Tasks.Task<TaskDto?> ITaskService.GetTaskByIdAsync(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting task {TaskId}", taskId);

        var task = await _context.Tasks
            .AsNoTracking()
            .Include(t => t.AssignedEmployee)
            .Include(t => t.CreatedByEmployee)
            .Include(t => t.TaskDocuments)
            .ThenInclude(td => td.Document)
            .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted, cancellationToken);

        return task == null ? null : MapTaskToDto(task);
    }

    async System.Threading.Tasks.Task<TaskDto> ITaskService.CreateTaskAsync(
        CreateTaskRequest request,
        Guid createdByUserId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating task '{Title}' assigned to employee {AssignedEmployeeId}",
            request.Title,
            request.AssignedEmployeeId);

        // Validate assigned employee exists
        var assignedEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.AssignedEmployeeId, cancellationToken);

        if (assignedEmployee == null)
        {
            throw new ArgumentException($"Employee with ID {request.AssignedEmployeeId} not found");
        }

        // Validate created by user exists
        var createdByEmployee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == createdByUserId, cancellationToken);

        if (createdByEmployee == null)
        {
            throw new ArgumentException($"User with ID {createdByUserId} not found");
        }

        var task = new Domain.Entities.TaskModel
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            SprintName = request.SprintName,
            Category = request.Category,
            Priority = request.Priority,
            Status = request.Status,
            StartDate = request.StartDate,
            DueDate = request.DueDate,
            AssignedEmployeeId = request.AssignedEmployeeId,
            CreatedByEmployeeId = createdByUserId,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow,
        };

        if (!task.IsValid())
        {
            throw new ArgumentException("Due date must be after or equal to start date");
        }

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task {TaskId} created successfully", task.Id);

        // Reload with navigation properties
        return await ((ITaskService)this).GetTaskByIdAsync(task.Id, cancellationToken) ?? throw new InvalidOperationException("Failed to retrieve created task");
    }

    async System.Threading.Tasks.Task<TaskDto?> ITaskService.UpdateTaskAsync(
        UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating task {TaskId}", request.Id);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == request.Id && !t.IsDeleted, cancellationToken);

        if (task == null)
        {
            return null;
        }

        // Update fields if provided
        if (!string.IsNullOrEmpty(request.Title))
            task.Title = request.Title;

        if (!string.IsNullOrEmpty(request.Description))
            task.Description = request.Description;

        if (!string.IsNullOrEmpty(request.SprintName))
            task.SprintName = request.SprintName;

        if (!string.IsNullOrEmpty(request.Category))
            task.Category = request.Category;

        if (!string.IsNullOrEmpty(request.Priority))
            task.Priority = request.Priority;

        if (!string.IsNullOrEmpty(request.Status))
        {
            task.Status = request.Status;

            // Set completed date if marking as completed
            if (request.Status == "Completed" && !task.CompletedDate.HasValue)
            {
                task.CompletedDate = DateTime.UtcNow;
            }
        }

        if (request.DueDate.HasValue)
            task.DueDate = request.DueDate.Value;

        if (request.AssignedEmployeeId.HasValue)
        {
            // Validate assigned employee exists
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.AssignedEmployeeId, cancellationToken);

            if (employee == null)
            {
                throw new ArgumentException($"Employee with ID {request.AssignedEmployeeId} not found");
            }

            task.AssignedEmployeeId = request.AssignedEmployeeId.Value;
        }

        if (!string.IsNullOrEmpty(request.Notes))
            task.Notes = request.Notes;

        task.UpdatedAt = DateTime.UtcNow;

        if (!task.IsValid())
        {
            throw new ArgumentException("Due date must be after or equal to start date");
        }

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task {TaskId} updated successfully", task.Id);

        return await ((ITaskService)this).GetTaskByIdAsync(task.Id, cancellationToken);
    }

    async System.Threading.Tasks.Task<TaskDto?> ITaskService.UpdateTaskStatusAsync(
        Guid taskId,
        string newStatus,
        Guid userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating status for task {TaskId} to {Status}", taskId, newStatus);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted, cancellationToken);

        if (task == null)
        {
            return null;
        }

        // Authorization: employees can only update their assigned tasks
        if (task.AssignedEmployeeId != userId)
        {
            throw new UnauthorizedAccessException("You can only update status of tasks assigned to you");
        }

        task.Status = newStatus;

        // Set completed date if marking as completed
        if (newStatus == "Completed" && !task.CompletedDate.HasValue)
        {
            task.CompletedDate = DateTime.UtcNow;
        }

        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task {TaskId} status updated to {Status}", taskId, newStatus);

        return await ((ITaskService)this).GetTaskByIdAsync(taskId, cancellationToken);
    }

    async System.Threading.Tasks.Task<bool> ITaskService.DeleteTaskAsync(
        Guid taskId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting task {TaskId}", taskId);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted, cancellationToken);

        if (task == null)
        {
            return false;
        }

        // Soft delete
        task.IsDeleted = true;
        task.UpdatedAt = DateTime.UtcNow;

        _context.Tasks.Update(task);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task {TaskId} deleted successfully", taskId);
        return true;
    }

    async System.Threading.Tasks.Task<TaskDashboardSummaryDto> ITaskService.GetTaskDashboardSummaryAsync(
        Guid? employeeId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting task dashboard summary for employee {EmployeeId}", employeeId);

        var query = _context.Tasks
            .Where(t => !t.IsDeleted);

        if (employeeId.HasValue)
        {
            query = query.Where(t => t.AssignedEmployeeId == employeeId);
        }

        var tasks = await query
            .Include(t => t.AssignedEmployee)
            .Include(t => t.CreatedByEmployee)
            .ToListAsync(cancellationToken);

        var totalTasks = tasks.Count;
        var completedTasks = tasks.Count(t => t.Status == "Completed");
        var overdueTasks = tasks.Count(t =>
            t.DueDate < DateTime.UtcNow && t.Status != "Completed");

        var statusSummary = new Dictionary<string, int>
        {
            { "To Do", tasks.Count(t => t.Status == "To Do") },
            { "In Progress", tasks.Count(t => t.Status == "In Progress") },
            { "On Hold", tasks.Count(t => t.Status == "On Hold") },
            { "Completed", completedTasks }
        };

        var recentTasks = tasks
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .Select(MapTaskToListItemDto)
            .ToList();

        return new TaskDashboardSummaryDto
        {
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            OverdueTasks = overdueTasks,
            StatusSummary = statusSummary,
            RecentTasks = recentTasks
        };
    }

    async System.Threading.Tasks.Task<TaskFilterOptionsDto> ITaskService.GetFilterOptionsAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting task filter options");

        var employees = await _context.Employees
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .Select(e => new EmployeeOptionDto
            {
                Id = e.Id,
                Name = $"{e.FirstName} {e.LastName}",
                Email = e.Email
            })
            .OrderBy(e => e.Name)
            .ToListAsync(cancellationToken);

        return new TaskFilterOptionsDto
        {
            Statuses = new[] { "To Do", "In Progress", "On Hold", "Completed" }.ToList(),
            Priorities = new[] { "Low", "Medium", "High" }.ToList(),
            Categories = new[] {
                "Development",
                "HR",
                "Finance",
                "Sales",
                "Training",
                "Other"
            }.ToList(),
            Sprints = new[] {
                "Sprint 22",
                "Sprint 23",
                "Sprint 24",
                "Sprint 25"
            }.ToList(),
            Employees = employees
        };
    }

    async System.Threading.Tasks.Task<bool> ITaskService.CanUserAccessTaskAsync(
        Guid taskId,
        Guid userId,
        string userRole,
        CancellationToken cancellationToken)
    {
        // Managers and admins can access all tasks
        if (userRole == RoleManager || userRole == RoleAdmin)
        {
            return true;
        }

        // Employees can only access their assigned tasks
        var task = await _context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted, cancellationToken);

        return task != null && task.AssignedEmployeeId == userId;
    }

    // ======== Helper Methods ========

    private IQueryable<Domain.Entities.TaskModel> ApplyFilters(IQueryable<Domain.Entities.TaskModel> query, FilterTasksRequest? filter)
    {
        if (filter == null)
        {
            return query.OrderBy(t => t.DueDate);
        }

        // Search by term
        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(t =>
                t.Title.ToLower().Contains(term) ||
                t.Description.ToLower().Contains(term) ||
                t.Category.ToLower().Contains(term));
        }

        // Filter by status
        if (filter.Statuses?.Count > 0)
        {
            query = query.Where(t => filter.Statuses.Contains(t.Status));
        }

        // Filter by priority
        if (filter.Priorities?.Count > 0)
        {
            query = query.Where(t => filter.Priorities.Contains(t.Priority));
        }

        // Filter by assigned employees
        if (filter.AssignedEmployeeIds?.Count > 0)
        {
            query = query.Where(t => filter.AssignedEmployeeIds.Contains(t.AssignedEmployeeId));
        }

        // Filter by category
        if (filter.Categories?.Count > 0)
        {
            query = query.Where(t => filter.Categories.Contains(t.Category));
        }

        // Filter by sprint
        if (filter.Sprints?.Count > 0)
        {
            query = query.Where(t => filter.Sprints.Contains(t.SprintName));
        }

        // Filter by date range
        if (filter.DateFrom.HasValue)
        {
            query = query.Where(t => t.DueDate >= filter.DateFrom);
        }

        if (filter.DateTo.HasValue)
        {
            query = query.Where(t => t.DueDate <= filter.DateTo);
        }

        // Sorting
        var sortBy = filter.SortBy ?? "DueDate";
        var isAscending = filter.SortOrder?.ToLower() == "asc";

        query = (sortBy, isAscending) switch
        {
            ("Priority", true) => query.OrderBy(t => t.Priority),
            ("Priority", false) => query.OrderByDescending(t => t.Priority),
            ("CreatedAt", true) => query.OrderBy(t => t.CreatedAt),
            ("CreatedAt", false) => query.OrderByDescending(t => t.CreatedAt),
            ("Title", true) => query.OrderBy(t => t.Title),
            ("Title", false) => query.OrderByDescending(t => t.Title),
            _ => isAscending
                ? query.OrderBy(t => t.DueDate)
                : query.OrderByDescending(t => t.DueDate)
        };

        return query;
    }

    private async System.Threading.Tasks.Task<PaginatedTaskResponse> GetPaginatedResponseAsync(
        IQueryable<Domain.Entities.TaskModel> query,
        FilterTasksRequest? filter,
        CancellationToken cancellationToken)
    {
        var pageNumber = filter?.PageNumber ?? 1;
        var pageSize = filter?.PageSize ?? 10;

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Include(t => t.AssignedEmployee)
            .Include(t => t.CreatedByEmployee)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedTaskResponse
        {
            Items = items.Select(MapTaskToListItemDto).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages
        };
    }

    private TaskDto MapTaskToDto(Domain.Entities.TaskModel task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            SprintName = task.SprintName,
            Category = task.Category,
            Priority = task.Priority,
            Status = task.Status,
            StartDate = task.StartDate,
            DueDate = task.DueDate,
            AssignedEmployeeId = task.AssignedEmployeeId,
            AssignedEmployeeName = task.AssignedEmployee != null
                ? $"{task.AssignedEmployee.FirstName} {task.AssignedEmployee.LastName}"
                : "Unknown",
            AssignedEmployeeEmail = task.AssignedEmployee?.Email ?? string.Empty,
            CreatedByEmployeeId = task.CreatedByEmployeeId,
            CreatedByEmployeeName = task.CreatedByEmployee != null
                ? $"{task.CreatedByEmployee.FirstName} {task.CreatedByEmployee.LastName}"
                : "Unknown",
            CreatedByEmail = task.CreatedByEmployee?.Email ?? string.Empty,
            Notes = task.Notes,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            CompletedDate = task.CompletedDate,
            AttachedDocuments = task.TaskDocuments?.Select(td => new TaskDocumentDto
            {
                Id = td.Document?.Id ?? Guid.Empty,
                FileName = td.Document?.FileName ?? string.Empty,
                FileType = td.Document?.FileType ?? string.Empty,
                FileSize = td.Document?.FileSize ?? 0,
                UploadDate = td.Document?.UploadDate ?? DateTime.MinValue,
                DocumentUrl = td.Document?.DocumentUrl,
                DocumentPurpose = td.DocumentPurpose,
                IsPrimary = td.IsPrimary,
                UploadedByEmail = td.Document?.UploadedByEmail ?? string.Empty
            }).ToList() ?? new()
        };
    }

    private TaskListItemDto MapTaskToListItemDto(Domain.Entities.TaskModel task)
    {
        return new TaskListItemDto
        {
            Id = task.Id,
            Title = task.Title,
            AssignedEmployeeName = task.AssignedEmployee != null
                ? $"{task.AssignedEmployee.FirstName} {task.AssignedEmployee.LastName}"
                : "Unknown",
            Status = task.Status,
            Priority = task.Priority,
            DueDate = task.DueDate,
            Category = task.Category,
            SprintName = task.SprintName
        };
    }
}
