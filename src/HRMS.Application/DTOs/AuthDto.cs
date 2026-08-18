namespace HRMS.Application.DTOs;

/// <summary>
/// Login request with username and password.
/// </summary>
public class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Optional IP address (for audit, captured by API).
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Optional user agent (for audit, captured by API).
    /// </summary>
    public string? UserAgent { get; set; }
}

/// <summary>
/// Login response with access and refresh tokens.
/// </summary>
public class TokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; } // Access token lifetime in seconds (900 = 15 min)
    public string TokenType { get; set; } = "Bearer";

    /// <summary>
    /// Logged-in employee info for UI context.
    /// </summary>
    public EmployeeInfoDto? Employee { get; set; }
}

/// <summary>
/// Employee info embedded in token response.
/// </summary>
public class EmployeeInfoDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Refresh token request.
/// </summary>
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// JWT token validation result.
/// </summary>
public class TokenValidationResultDto
{
    public bool IsValid { get; set; }
    public Guid? EmployeeId { get; set; }
    public string[]? Roles { get; set; }
    public string? ErrorMessage { get; set; }
}
