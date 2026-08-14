namespace HRMS.Application.Contracts;

/// <summary>
/// Authentication service for managing authentication state and tokens.
/// Handles JWT token generation and validation for stateless API.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Generate a JWT token for authenticated user.
    /// Token includes user identity, roles, and permissions.
    /// </summary>
    Task<AuthenticationToken> GenerateTokenAsync(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate and decode a JWT token.
    /// Returns user information embedded in token.
    /// </summary>
    Task<TokenValidationResult?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh an expiring token (if refresh token mechanism is used).
    /// </summary>
    Task<AuthenticationToken?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke a token (logout).
    /// </summary>
    Task RevokeTokenAsync(string token, CancellationToken cancellationToken = default);
}

/// <summary>
/// JWT token details.
/// </summary>
public class AuthenticationToken
{
    public string AccessToken { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public int ExpiresInSeconds { get; set; } = 3600; // 1 hour default
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Result of token validation.
/// </summary>
public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
    public Dictionary<string, object>? Claims { get; set; }
    public string? ErrorMessage { get; set; }
}
