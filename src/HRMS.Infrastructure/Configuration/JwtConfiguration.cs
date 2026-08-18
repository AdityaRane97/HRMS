namespace HRMS.Infrastructure.Configuration;

/// <summary>
/// JWT authentication configuration.
/// Loaded from appsettings.json under "Jwt" section.
/// Phase 2.2: Uses appsettings; Phase 3 will migrate to Azure Key Vault.
/// </summary>
public class JwtConfiguration
{
    /// <summary>
    /// JWT signing secret key (HS256).
    /// Phase 2.2: Random 256-bit key for testing
    /// Phase 3: Move to Key Vault or environment variable
    /// NEVER commit real secrets to source control.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// JWT issuer claim (identifies token source).
    /// Standard: your app domain or identifier
    /// </summary>
    public string Issuer { get; set; } = "HRMSApp";

    /// <summary>
    /// JWT audience claim (identifies intended recipients).
    /// Standard: your app name or domain
    /// </summary>
    public string Audience { get; set; } = "HRMSClient";

    /// <summary>
    /// Access token expiration time in minutes.
    /// Phase 2.2: 15 minutes (standard for short-lived tokens)
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Refresh token expiration time in days.
    /// Phase 2.2: 7 days (standard for refresh tokens)
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;

    /// <summary>
    /// Validate JWT configuration on startup.
    /// Throws if essential fields are missing.
    /// </summary>
    public void Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(SecretKey) || SecretKey.Length < 32)
            errors.Add("JWT SecretKey must be at least 32 characters");

        if (string.IsNullOrWhiteSpace(Issuer))
            errors.Add("JWT Issuer is required");

        if (string.IsNullOrWhiteSpace(Audience))
            errors.Add("JWT Audience is required");

        if (AccessTokenExpirationMinutes <= 0)
            errors.Add("AccessTokenExpirationMinutes must be greater than 0");

        if (RefreshTokenExpirationDays <= 0)
            errors.Add("RefreshTokenExpirationDays must be greater than 0");

        if (errors.Count > 0)
            throw new InvalidOperationException($"JWT Configuration validation failed:\n{string.Join("\n", errors)}");
    }
}
