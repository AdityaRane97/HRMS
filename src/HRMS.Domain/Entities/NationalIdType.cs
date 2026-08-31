using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Master data for types of national identifiers
/// (Aadhaar, PAN, Passport, DrivingLicense, VoterId, etc.)
/// </summary>
public class NationalIdType : BaseEntity
{
    /// <summary>
    /// Name of the ID type
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Code/short identifier
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Country for which this ID type is valid
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Expected format or validation pattern for this ID type
    /// </summary>
    public string? ValidationPattern { get; set; }

    /// <summary>
    /// Whether this ID type has an expiry date
    /// </summary>
    public bool HasExpiry { get; set; }

    /// <summary>
    /// Sort order for displaying in UI
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Whether this ID type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    public NationalIdType()
    {
    }

    public NationalIdType(string name, string code, string country)
    {
        Name = name;
        Code = code;
        Country = country;
    }
}
