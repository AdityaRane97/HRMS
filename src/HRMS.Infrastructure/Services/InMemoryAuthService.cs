using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HRMS.Application.DTOs;
using HRMS.Application.Services;
using HRMS.Domain.Entities;
using HRMS.Infrastructure.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HRMS.Infrastructure.Services;

/// <summary>
/// In-memory authentication service.
/// Phase 2.2: JWT (15 min) + Refresh (7 days), minimal claims, in-memory token storage.
/// Phase 3: Replace in-memory refresh token store with database (add RefreshTokenDbConfiguration).
/// </summary>
public class InMemoryAuthService : IAuthService
{
    // Phase 2.2: In-memory refresh token storage.
    // Phase 3: Replace with DbSet<RefreshToken> from HrmsDbContext.
    private static readonly Dictionary<string, RefreshToken> RefreshTokenStore = new();

    private readonly JwtConfiguration _jwtConfig;
    private readonly IEmployeeService _employeeService;

    public InMemoryAuthService(JwtConfiguration jwtConfig, IEmployeeService employeeService)
    {
        _jwtConfig = jwtConfig ?? throw new ArgumentNullException(nameof(jwtConfig));
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
        _jwtConfig.Validate(); // Fail fast if config is invalid
    }

    /// <summary>
    /// Authenticate user with username/password and issue JWT + refresh token.
    /// Phase 2.2: Uses IEmployeeService to validate credentials.
    /// Passwords are hashed (assumed in Employee entity).
    /// Returns null if credentials invalid.
    /// </summary>
    public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return null;

        // Phase 2.2: Fetch employee by username/email.
        // Assumes Employee has Username property and password hashing method.
        var employee = await _employeeService.GetEmployeeByUsernameAsync(request.Username);

        if (employee == null)
            return null; // Username not found

        // Phase 2.2: Validate password (assumes VerifyPassword or similar on Employee).
        // For now, plain-string comparison; Phase 3 will use BCrypt hashing.
        if (!VerifyPassword(request.Password, employee.PasswordHash))
            return null; // Password mismatch

        // Generate access token (15 min)
        var accessToken = GenerateAccessToken(employee);

        // Generate and store refresh token (7 days)
        var refreshToken = GenerateRefreshToken(employee, request.IpAddress, request.UserAgent);
        RefreshTokenStore[refreshToken.Token] = refreshToken;

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = _jwtConfig.AccessTokenExpirationMinutes * 60, // Convert to seconds
            TokenType = "Bearer",
            Employee = new EmployeeInfoDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Role = "User" // TODO: Phase 3 - Get from UserRole junction table
            }
        };
    }

    /// <summary>
    /// Validate an access JWT token.
    /// Phase 2.2: Validates signature, expiration, issuer, and audience.
    /// Returns validation result with EmployeeId and roles.
    /// </summary>
    public async Task<TokenValidationResultDto> ValidateTokenAsync(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);

            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtConfig.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // No clock skew tolerance
            }, out SecurityToken validatedToken);

            var employeeIdClaim = principal.FindFirst("sub")?.Value;
            if (!Guid.TryParse(employeeIdClaim, out var employeeId))
                return new TokenValidationResultDto { IsValid = false, ErrorMessage = "Invalid employee ID in token" };

            var rolesClaim = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

            return new TokenValidationResultDto
            {
                IsValid = true,
                EmployeeId = employeeId,
                Roles = rolesClaim
            };
        }
        catch (Exception ex)
        {
            return new TokenValidationResultDto
            {
                IsValid = false,
                ErrorMessage = $"Token validation failed: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Issue a new access token using a valid refresh token.
    /// Validates refresh token and checks expiration/revocation.
    /// Re-uses refresh token for same session (no rotation by default).
    /// Phase 3: Can implement refresh token rotation here.
    /// </summary>
    public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return null;

        // Phase 2.2: Look up refresh token in in-memory store
        if (!RefreshTokenStore.TryGetValue(request.RefreshToken, out var storedToken))
            return null; // Token not found in store

        // Validate refresh token (not expired, not revoked)
        if (!storedToken.IsValid)
            return null; // Token expired or revoked

        // Load employee info for new access token
        var employee = await _employeeService.GetEmployeeByIdAsync(storedToken.EmployeeId);
        if (employee == null)
            return null; // Employee not found (cleanup opportunity in Phase 3)

        // Mark refresh token as used (for activity tracking)
        storedToken.MarkAsUsed();

        // Generate new access token
        var accessToken = GenerateAccessToken(employee);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = request.RefreshToken, // Re-use refresh token
            ExpiresIn = _jwtConfig.AccessTokenExpirationMinutes * 60,
            TokenType = "Bearer",
            Employee = new EmployeeInfoDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Role = "User" // TODO: Phase 3 - Get from UserRole junction table
            }
        };
    }

    /// <summary>
    /// Revoke a refresh token (mark as revoked in in-memory store).
    /// Used for logout or security events.
    /// Phase 3: Mark deleted in database for persistence across restarts.
    /// </summary>
    public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, string reason = "User logout")
    {
        if (!RefreshTokenStore.TryGetValue(refreshToken, out var storedToken))
            return false;

        storedToken.Revoke(reason);
        return true;
    }

    /// <summary>
    /// Validate refresh token status (check if not expired and not revoked).
    /// Used in token lifecycle checks.
    /// </summary>
    public async Task<bool> ValidateRefreshTokenAsync(string token)
    {
        if (!RefreshTokenStore.TryGetValue(token, out var storedToken))
            return false;

        return storedToken.IsValid;
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    /// <summary>
    /// Generate access JWT token (15 min expiry, minimal claims).
    /// Claims: sub (employee ID), role, permissions.
    /// Phase 2.2: Minimal claims per spec.
    /// Phase 3: Add more claims (department, job title, etc.) if needed.
    /// </summary>
    private string GenerateAccessToken(Employee employee)
    {
        var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_jwtConfig.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new Claim("sub", employee.Id.ToString()), // Subject (employee ID)
            new Claim(ClaimTypes.Name, $"{employee.FirstName} {employee.LastName}"),
            new Claim(ClaimTypes.Email, employee.Email),
            // TODO: Phase 3 - Add roles and permissions from UserRole junction table
            // new Claim(ClaimTypes.Role, employee.Role?.Name ?? "User")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = _jwtConfig.Issuer,
            Audience = _jwtConfig.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }

    /// <summary>
    /// Generate a new refresh token (7 day expiry).
    /// Phase 2.2: In-memory storage with minimal data.
    /// Phase 3: Persist to database with encrypted token storage.
    /// </summary>
    private RefreshToken GenerateRefreshToken(Employee employee, string? ipAddress = null, string? userAgent = null)
    {
        var randomBytes = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        var token = new RefreshToken(employee.Id, Convert.ToBase64String(randomBytes))
        {
            IssuedFromIpAddress = ipAddress,
            IssuedFromUserAgent = userAgent
        };

        return token;
    }

    /// <summary>
    /// Verify password against stored hash.
    /// Phase 2.2: Plain comparison (for testing only; NOT production-safe).
    /// Phase 3: Replace with BCrypt.Net-Next hashing library.
    /// </summary>
    private bool VerifyPassword(string password, string? passwordHash)
    {
        // Phase 2.2 TEMP: Plain string comparison for testing
        // WARNING: This is NOT secure!
        // Phase 3: Replace with:
        // return BCrypt.Net.BCrypt.Verify(password, passwordHash);

        if (string.IsNullOrEmpty(passwordHash))
            return false;

        return password == passwordHash;
    }
}
