using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents a national identifier (Aadhaar, PAN, Passport, License, etc.) for an employee.
/// Sensitive information should be masked when displayed.
/// </summary>
public class NationalIdentifier : BaseEntity
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
    /// Country for which the identifier is valid
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Type of national identifier (Aadhaar, PAN, Passport, DrivingLicense, VoterId, etc.)
    /// </summary>
    public string IdType { get; set; } = string.Empty;

    /// <summary>
    /// The actual identifier number. Should be stored securely and masked when displayed.
    /// </summary>
    public string IdNumber { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this is the primary national identifier
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Issue date of the identifier
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// Expiry date of the identifier (null if no expiry)
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Remarks or additional information about the identifier
    /// </summary>
    public string? Remarks { get; set; }

    public NationalIdentifier()
    {
    }

    public NationalIdentifier(Guid employeeId, string country, string idType, string idNumber, bool isPrimary = false)
    {
        EmployeeId = employeeId;
        Country = country;
        IdType = idType;
        IdNumber = idNumber;
        IsPrimary = isPrimary;
    }

    /// <summary>
    /// Check if the identifier has expired
    /// </summary>
    public bool IsExpired() => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;

    /// <summary>
    /// Get a masked version of the ID number for display (show last 4 characters only)
    /// </summary>
    public string GetMaskedIdNumber()
    {
        if (IdNumber.Length <= 4)
            return "•".PadRight(IdNumber.Length, '•');

        var visibleChars = IdNumber.Length - 4;
        return "•".PadRight(IdNumber.Length - visibleChars, '•') + IdNumber.Substring(IdNumber.Length - 4);
    }
}
