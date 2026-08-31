using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Master data for types of relationships in family contacts
/// (Spouse, Father, Mother, Child, Sibling, etc.)
/// </summary>
public class RelationshipType : BaseEntity
{
    /// <summary>
    /// Name of the relationship type
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
    /// Sort order for displaying in UI
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Whether this relationship type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    public RelationshipType()
    {
    }

    public RelationshipType(string name, string code)
    {
        Name = name;
        Code = code;
    }
}
