using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents an employee in the HRMS.
/// Core aggregate root managing employee information, employment details, and relationships.
/// </summary>
public class Employee : AggregateRoot
{
    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? EmployeeDepartment { get; set; }
    public string? Designation { get; set; }

    // Employment Information
    public string EmployeeCode { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public DateTime? ResignationDate { get; set; }
    public string EmploymentStatus { get; set; } = "Active"; // Active, OnLeave, Resigned, Retired
    public string EmploymentType { get; set; } = "FullTime"; // FullTime, PartTime, Contract, Intern

    // Reporting Hierarchy
    public Guid? ManagerId { get; set; }
    public virtual Employee? Manager { get; set; }
    public virtual ICollection<Employee> DirectReports { get; set; } = [];

    // Organization
    public Guid OrganizationId { get; set; }
    public virtual Organization? Organization { get; set; }

    // Authentication & Identity
    public string? IdentityProvider { get; set; } // "AzureAD", "Okta", "Custom", etc.
    public string? ExternalUserId { get; set; } // ID from external identity provider
    public bool IsActive { get; set; } = true;

    // Additional Fields
    public string? ProfilePhotoUrl { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }

    public Employee()
    {
    }

    public Employee(
        string firstName,
        string lastName,
        string email,
        string employeeCode,
        DateTime joinDate,
        Guid organizationId)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        EmployeeCode = employeeCode;
        JoinDate = joinDate;
        OrganizationId = organizationId;
    }

    /// <summary>
    /// Set the direct manager for this employee.
    /// </summary>
    /// <param name="manager">The manager employee</param>
    public void SetManager(Employee? manager)
    {
        ManagerId = manager?.Id;
        Manager = manager;
    }

    /// <summary>
    /// Get full name of the employee.
    /// </summary>
    public string GetFullName() => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Check if employee is actively employed.
    /// </summary>
    public bool IsCurrentlyEmployed() 
        => IsActive && EmploymentStatus == "Active" && ResignationDate is null;
}
