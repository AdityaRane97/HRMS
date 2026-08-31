using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents a family member or emergency contact for an employee.
/// </summary>
public class FamilyContact : BaseEntity
{
    /// <summary>
    /// Foreign key to Employee
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Navigation property to Employee
    /// </summary>
    public virtual Employee? Employee { get; set; }

    /// <summary>
    /// Name of the family member/contact
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Relationship to the employee (Spouse, Father, Mother, Child, Sibling, etc.)
    /// </summary>
    public string Relationship { get; set; } = string.Empty;

    /// <summary>
    /// Phone number of the contact
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Email address of the contact
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Date of birth of the family member
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Address of the family member
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Indicates if this is an emergency contact
    /// </summary>
    public bool IsEmergencyContact { get; set; }

    /// <summary>
    /// Indicates if this is the primary contact for emergencies
    /// </summary>
    public bool IsPrimaryContact { get; set; }

    /// <summary>
    /// Remarks or additional information about the contact
    /// </summary>
    public string? Remarks { get; set; }

    public FamilyContact()
    {
    }

    public FamilyContact(Guid employeeId, string contactName, string relationship, bool isEmergencyContact = false)
    {
        EmployeeId = employeeId;
        ContactName = contactName;
        Relationship = relationship;
        IsEmergencyContact = isEmergencyContact;
    }

    /// <summary>
    /// Get age of the contact based on date of birth
    /// </summary>
    public int? GetAge()
    {
        if (!DateOfBirth.HasValue)
            return null;

        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Value.Year;
        if (DateOfBirth.Value.Date > today.AddYears(-age))
            age--;

        return age;
    }
}
