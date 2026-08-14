namespace HRMS.Application.DTOs;

/// <summary>
/// DTO for Employee data exposed in APIs.
/// Does not expose sensitive internal fields.
/// </summary>
public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public string EmploymentStatus { get; set; } = "Active";
    public string EmploymentType { get; set; } = "FullTime";
    public Guid? ManagerId { get; set; }
    public Guid OrganizationId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ProfilePhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public string GetFullName() => $"{FirstName} {LastName}".Trim();
}

/// <summary>
/// DTO for creating an employee.
/// </summary>
public class CreateEmployeeDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public string EmploymentType { get; set; } = "FullTime";
    public Guid? ManagerId { get; set; }
    public Guid OrganizationId { get; set; }
}

/// <summary>
/// DTO for updating an employee.
/// </summary>
public class UpdateEmployeeDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? Designation { get; set; }
    public string? EmploymentStatus { get; set; }
    public Guid? ManagerId { get; set; }
}
