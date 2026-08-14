namespace HRMS.Application.Contracts;

/// <summary>
/// Identity provider abstraction for cloud-neutral authentication.
/// Supports Azure AD, Okta, custom SSO, or any external identity provider.
/// The implementation is deferred until client provides SSO details.
/// </summary>
public interface IIdentityProvider
{
    /// <summary>
    /// Authenticate a user and return their identity information.
    /// </summary>
    Task<IdentityResult?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate an external identity token (JWT, SAML, etc).
    /// </summary>
    Task<IdentityResult?> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user information from external identity provider.
    /// </summary>
    Task<ExternalUserInfo?> GetUserInfoAsync(string externalUserId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of identity provider authentication.
/// </summary>
public class IdentityResult
{
    public bool Success { get; set; }
    public string? ExternalUserId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public Dictionary<string, object>? Claims { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// User information from external identity provider.
/// </summary>
public class ExternalUserInfo
{
    public string ExternalUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public Dictionary<string, object>? Claims { get; set; }
}
