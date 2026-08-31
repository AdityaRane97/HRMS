using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents the leave/absence balance for an employee for a specific leave type.
/// Tracks available, used, and accrued leave balance.
/// </summary>
public class LeaveBalance : BaseEntity
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
    /// Foreign key to LeaveType
    /// </summary>
    public Guid LeaveTypeId { get; set; }

    /// <summary>
    /// Navigation property to LeaveType
    /// </summary>
    public virtual LeaveType? LeaveType { get; set; }

    /// <summary>
    /// Available balance for the leave type
    /// </summary>
    public decimal AvailableBalance { get; set; }

    /// <summary>
    /// Already used balance for the leave type
    /// </summary>
    public decimal UsedBalance { get; set; }

    /// <summary>
    /// Accrued balance for the leave type (for the year/period)
    /// </summary>
    public decimal AccruedBalance { get; set; }

    /// <summary>
    /// Carry forward balance from previous period
    /// </summary>
    public decimal CarryForwardBalance { get; set; }

    /// <summary>
    /// Last date when the balance was calculated/updated
    /// </summary>
    public DateTime LastCalculatedDate { get; set; }

    /// <summary>
    /// Financial year or period for which this balance applies
    /// </summary>
    public string FinancialYear { get; set; } = string.Empty;

    /// <summary>
    /// Remarks about the balance
    /// </summary>
    public string? Remarks { get; set; }

    public LeaveBalance()
    {
    }

    public LeaveBalance(Guid employeeId, Guid leaveTypeId, decimal accruedBalance)
    {
        EmployeeId = employeeId;
        LeaveTypeId = leaveTypeId;
        AccruedBalance = accruedBalance;
        AvailableBalance = accruedBalance;
        UsedBalance = 0;
        CarryForwardBalance = 0;
        LastCalculatedDate = DateTime.UtcNow;
        FinancialYear = DateTime.UtcNow.Year.ToString();
    }

    /// <summary>
    /// Deduct balance when leave is approved
    /// </summary>
    public void DeductBalance(decimal amount)
    {
        if (amount > AvailableBalance)
            throw new InvalidOperationException($"Insufficient leave balance. Available: {AvailableBalance}, Requested: {amount}");

        AvailableBalance -= amount;
        UsedBalance += amount;
    }

    /// <summary>
    /// Add balance back when leave is cancelled
    /// </summary>
    public void AddBalance(decimal amount)
    {
        AvailableBalance += amount;
        UsedBalance -= amount;
    }

    /// <summary>
    /// Check if there is sufficient balance for the requested amount
    /// </summary>
    public bool HasSufficientBalance(decimal requestedDays) => AvailableBalance >= requestedDays;

    /// <summary>
    /// Calculate remaining balance
    /// </summary>
    public decimal GetRemainingBalance() => AvailableBalance;
}
