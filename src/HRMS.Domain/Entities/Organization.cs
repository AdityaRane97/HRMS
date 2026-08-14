using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents an organization/company.
/// Container for employees and organizational hierarchy.
/// </summary>
public class Organization : AggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    // Relationships
    public virtual ICollection<Employee> Employees { get; set; } = [];

    public Organization()
    {
    }

    public Organization(string name, string registrationNumber)
    {
        Name = name;
        RegistrationNumber = registrationNumber;
    }
}
