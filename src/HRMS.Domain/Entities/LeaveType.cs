using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Master data for leave types (Annual, Sick, Personal Care, Maternity, etc.)
/// </summary>
public class LeaveType : BaseEntity
{
    /// <summary>
    /// Name of the leave type
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Code/short identifier for the leave type
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Description of the leave type
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Default allocation for this leave type per year
    /// </summary>
    public decimal DefaultAllocation { get; set; }

    /// <summary>
    /// Unit for this leave type (Days or Hours)
    /// </summary>
    public string Unit { get; set; } = "Days"; // Days, Hours

    /// <summary>
    /// Whether carry forward is allowed for this leave type
    /// </summary>
    public bool AllowCarryForward { get; set; }

    /// <summary>
    /// Maximum carry forward amount if allowed
    /// </summary>
    public decimal? MaxCarryForwardAmount { get; set; }

    /// <summary>
    /// Whether this leave type requires approval from manager
    /// </summary>
    public bool RequiresManagerApproval { get; set; } = true;

    /// <summary>
    /// Whether this leave type requires approval from HR
    /// </summary>
    public bool RequiresHRApproval { get; set; }

    /// <summary>
    /// Whether this leave type is paid or unpaid
    /// </summary>
    public bool IsPaid { get; set; } = true;

    /// <summary>
    /// Whether this leave type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Sort order for displaying in UI
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Navigation property for leave balances
    /// </summary>
    public virtual ICollection<LeaveBalance> LeaveBalances { get; set; } = [];

    public LeaveType()
    {
    }

    public LeaveType(string name, string code, string description, decimal defaultAllocation)
    {
        Name = name;
        Code = code;
        Description = description;
        DefaultAllocation = defaultAllocation;
    }
}
