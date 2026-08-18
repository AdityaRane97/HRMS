using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents a refresh token for JWT token renewal.
/// Tracks token lifecycle, expiration, and revocation status.
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// The actual refresh token string (unique, base64-encoded).
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Employee ID who owns this refresh token.
    /// </summary>
    public Guid EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    /// <summary>
    /// Token expiration date/time.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether this refresh token has been revoked.
    /// </summary>
    public bool IsRevoked { get; set; } = false;

    /// <summary>
    /// Optional revocation reason (for audit trail).
    /// </summary>
    public string? RevocationReason { get; set; }

    /// <summary>
    /// IP address from which token was created (for security audit).
    /// </summary>
    public string? IssuedFromIpAddress { get; set; }

    /// <summary>
    /// User agent from which token was created (for security audit).
    /// </summary>
    public string? IssuedFromUserAgent { get; set; }

    /// <summary>
    /// Last used timestamp (for activity tracking).
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    public RefreshToken()
    {
    }

    public RefreshToken(Guid employeeId, string token)
    {
        EmployeeId = employeeId;
        Token = token;
        ExpiresAt = DateTime.UtcNow.AddDays(7); // 7-day refresh token expiry
    }

    /// <summary>
    /// Check if refresh token is still valid (not expired and not revoked).
    /// </summary>
    public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;

    /// <summary>
    /// Check if refresh token has expired.
    /// </summary>
    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;

    /// <summary>
    /// Revoke this refresh token.
    /// </summary>
    public void Revoke(string reason = "")
    {
        IsRevoked = true;
        RevocationReason = reason;
    }

    /// <summary>
    /// Mark last used timestamp (for activity tracking).
    /// </summary>
    public void MarkAsUsed()
    {
        LastUsedAt = DateTime.UtcNow;
    }
}
