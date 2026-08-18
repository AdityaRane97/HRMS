using HRMS.Application.DTOs;

namespace HRMS.Application.Services;

/// <summary>
/// Authentication service contract.
/// Handles JWT generation, refresh token lifecycle, and validation.
/// Phase 2.2: JWT (15 min) + Refresh (7 days), minimal claims, in-memory token storage.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Authenticate user with username/password and issue JWT + refresh token.
    /// Returns TokenResponseDto on success, null if credentials invalid.
    /// Captures IP address and User-Agent for audit trail.
    /// </summary>
    Task<TokenResponseDto?> LoginAsync(LoginRequestDto request);

    /// <summary>
    /// Validate an access JWT token.
    /// Returns validation result with EmployeeId, roles, and error message if invalid.
    /// Phase 2.2: Validates signature, expiration, issuer, and audience.
    /// </summary>
    Task<TokenValidationResultDto> ValidateTokenAsync(string token);

    /// <summary>
    /// Issue a new access token using a valid refresh token.
    /// Returns TokenResponseDto on success, null if refresh token invalid/expired/revoked.
    /// </summary>
    Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);

    /// <summary>
    /// Revoke a refresh token (mark as revoked in in-memory store).
    /// Used for logout or security reasons.
    /// </summary>
    Task<bool> RevokeRefreshTokenAsync(string refreshToken, string reason = "User logout");

    /// <summary>
    /// Validate refresh token status (not expired, not revoked, associated with employee).
    /// Returns true if valid, false otherwise.
    /// </summary>
    Task<bool> ValidateRefreshTokenAsync(string token);
}
