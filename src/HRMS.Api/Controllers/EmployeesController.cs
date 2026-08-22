using AutoMapper;
using HRMS.Application.Constants;
using HRMS.Application.DTOs;
using HRMS.Domain.Entities;
using HRMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Api.Controllers;

/// <summary>
/// Employee management API endpoints.
/// Supports CRUD operations with role-based authorization.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly HrmsDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(
        HrmsDbContext context,
        IMapper mapper,
        ILogger<EmployeesController> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Get all employees (paginated).
    /// Accessible only to HR and Admin users.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpGet]
    [ProducesResponseType(
        typeof(ApiResponse<PaginatedList<EmployeeDto>>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<PaginatedList<EmployeeDto>>>> GetEmployees(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting employees - Page {PageNumber}, Size {PageSize}",
            pageNumber,
            pageSize);

        var query = _context.Employees.AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var employees = await query
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var employeeDtos = _mapper.Map<List<EmployeeDto>>(employees);

        var paginatedList = new PaginatedList<EmployeeDto>
        {
            Items = employeeDtos,
            Pagination = new PaginationMetadata
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        };

        return Ok(new ApiResponse<PaginatedList<EmployeeDto>>
        {
            Success = true,
            Data = paginatedList,
            Message = "Employees retrieved successfully",
            TraceId = HttpContext.TraceIdentifier
        });
    }

    /// <summary>
    /// Get employee by ID.
    /// Requires authentication.
    /// Resource-level ownership and manager hierarchy validation
    /// will be added in the next RBAC step.
    /// </summary>
    [Authorize]
    [HttpGet("{id}")]
    [ProducesResponseType(
        typeof(ApiResponse<EmployeeDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetEmployee(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Getting employee {EmployeeId}",
            id);

        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.Id == id,
                cancellationToken);

        if (employee == null)
        {
            _logger.LogWarning(
                "Employee {EmployeeId} not found",
                id);

            return NotFound(new ProblemDetails
            {
                Type = "https://hrms.example.com/errors/notfound",
                Title = "Employee Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Employee with ID '{id}' not found.",
                Instance = HttpContext.Request.Path
            });
        }

        var employeeDto = _mapper.Map<EmployeeDto>(employee);

        return Ok(new ApiResponse<EmployeeDto>
        {
            Success = true,
            Data = employeeDto,
            Message = "Employee retrieved successfully",
            TraceId = HttpContext.TraceIdentifier
        });
    }

    /// <summary>
    /// Create a new employee.
    /// Accessible only to HR and Admin users.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpPost]
    [ProducesResponseType(
        typeof(ApiResponse<EmployeeDto>),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> CreateEmployee(
        CreateEmployeeDto createDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Creating employee {Email}",
            createDto.Email);

        var existingEmployee = await _context.Employees
            .FirstOrDefaultAsync(
                e => e.Email == createDto.Email,
                cancellationToken);

        if (existingEmployee != null)
        {
            return BadRequest(new ProblemDetails
            {
                Type = "https://hrms.example.com/errors/validation",
                Title = "Validation Failed",
                Status = StatusCodes.Status400BadRequest,
                Detail =
                    $"Employee with email '{createDto.Email}' already exists."
            });
        }

        var employee = _mapper.Map<Employee>(createDto);

        employee.Id = Guid.NewGuid();
        employee.CreatedAt = DateTime.UtcNow;

        _context.Employees.Add(employee);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Employee {EmployeeId} created successfully",
            employee.Id);

        var employeeDto = _mapper.Map<EmployeeDto>(employee);

        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = employee.Id },
            new ApiResponse<EmployeeDto>
            {
                Success = true,
                Data = employeeDto,
                Message = "Employee created successfully",
                TraceId = HttpContext.TraceIdentifier
            });
    }

    /// <summary>
    /// Update an employee.
    /// Accessible only to HR and Admin users.
    /// </summary>
    [Authorize(Roles = RoleConstants.HROrAdmin)]
    [HttpPut("{id}")]
    [ProducesResponseType(
        typeof(ApiResponse<EmployeeDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> UpdateEmployee(
        Guid id,
        UpdateEmployeeDto updateDto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Updating employee {EmployeeId}",
            id);

        var employee = await _context.Employees
            .FirstOrDefaultAsync(
                e => e.Id == id,
                cancellationToken);

        if (employee == null)
        {
            return NotFound(new ProblemDetails
            {
                Type = "https://hrms.example.com/errors/notfound",
                Title = "Employee Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = $"Employee with ID '{id}' not found."
            });
        }

        _mapper.Map(updateDto, employee);

        employee.UpdatedAt = DateTime.UtcNow;

        _context.Employees.Update(employee);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Employee {EmployeeId} updated successfully",
            id);

        var employeeDto = _mapper.Map<EmployeeDto>(employee);

        return Ok(new ApiResponse<EmployeeDto>
        {
            Success = true,
            Data = employeeDto,
            Message = "Employee updated successfully",
            TraceId = HttpContext.TraceIdentifier
        });
    }

    /// <summary>
    /// Get current authenticated user's employee profile.
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(
        typeof(ApiResponse<EmployeeDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EmployeeDto>>> GetCurrentEmployee(
        CancellationToken cancellationToken = default)
    {
        var currentUser = HttpContext.User;

        _logger.LogInformation(
            "Getting current user employee profile");

        var userId = Guid.TryParse(
            currentUser.FindFirst("sub")?.Value,
            out var parsedId)
            ? parsedId
            : Guid.Empty;

        if (userId == Guid.Empty)
        {
            return Unauthorized(new ProblemDetails
            {
                Type = "https://hrms.example.com/errors/unauthorized",
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "User is not authenticated."
            });
        }

        var employee = await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(
                e => e.Id == userId,
                cancellationToken);

        if (employee == null)
        {
            return NotFound(new ProblemDetails
            {
                Type = "https://hrms.example.com/errors/notfound",
                Title = "Employee Profile Not Found",
                Status = StatusCodes.Status404NotFound,
                Detail = "Your employee profile could not be found."
            });
        }

        var employeeDto = _mapper.Map<EmployeeDto>(employee);

        return Ok(new ApiResponse<EmployeeDto>
        {
            Success = true,
            Data = employeeDto,
            Message = "Current employee profile retrieved successfully",
            TraceId = HttpContext.TraceIdentifier
        });
    }
}