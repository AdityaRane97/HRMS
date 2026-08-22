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
/// Phase 2.3: JWT role claims added for Role-Based Access Control.
/// Phase 3: Replace in-memory refresh token store with database.
/// </summary>
public class InMemoryAuthService : IAuthService
{
    private static readonly Dictionary<string, RefreshToken> RefreshTokenStore = new();

    private readonly JwtConfiguration _jwtConfig;
    private readonly IEmployeeService _employeeService;

    public InMemoryAuthService(
        JwtConfiguration jwtConfig,
        IEmployeeService employeeService)
    {
        _jwtConfig = jwtConfig ?? throw new ArgumentNullException(nameof(jwtConfig));
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));

        _jwtConfig.Validate();
    }

    /// <summary>
    /// Authenticate user with username/password and issue JWT + refresh token.
    /// </summary>
    public async Task<TokenResponseDto?> LoginAsync(LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var employee =
            await _employeeService.GetEmployeeByUsernameAsync(request.Username);

        if (employee == null)
        {
            return null;
        }

        if (!VerifyPassword(request.Password, employee.PasswordHash))
        {
            return null;
        }

        var role = GetEmployeeRole(employee);

        var accessToken = GenerateAccessToken(employee, role);

        var refreshToken = GenerateRefreshToken(
            employee,
            request.IpAddress,
            request.UserAgent);

        RefreshTokenStore[refreshToken.Token] = refreshToken;

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresIn = _jwtConfig.AccessTokenExpirationMinutes * 60,
            TokenType = "Bearer",
            Employee = new EmployeeInfoDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Role = role
            }
        };
    }

    /// <summary>
    /// Validate an access JWT token.
    /// </summary>
    public async Task<TokenValidationResultDto> ValidateTokenAsync(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);

            var principal = handler.ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ValidateIssuer = true,
                    ValidIssuer = _jwtConfig.Issuer,

                    ValidateAudience = true,
                    ValidAudience = _jwtConfig.Audience,

                    ValidateLifetime = true,

                    RoleClaimType = ClaimTypes.Role,

                    ClockSkew = TimeSpan.Zero
                },
                out _);

            var employeeIdClaim = principal.FindFirst("sub")?.Value;

            if (!Guid.TryParse(employeeIdClaim, out var employeeId))
            {
                return new TokenValidationResultDto
                {
                    IsValid = false,
                    ErrorMessage = "Invalid employee ID in token"
                };
            }

            var roles = principal
                .FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
                .ToArray();

            return new TokenValidationResultDto
            {
                IsValid = true,
                EmployeeId = employeeId,
                Roles = roles
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
    /// </summary>
    public async Task<TokenResponseDto?> RefreshTokenAsync(
        RefreshTokenRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return null;
        }

        if (!RefreshTokenStore.TryGetValue(
                request.RefreshToken,
                out var storedToken))
        {
            return null;
        }

        if (!storedToken.IsValid)
        {
            return null;
        }

        var employee =
            await _employeeService.GetEmployeeByIdAsync(
                storedToken.EmployeeId);

        if (employee == null)
        {
            return null;
        }

        storedToken.MarkAsUsed();

        var role = GetEmployeeRole(employee);

        var accessToken = GenerateAccessToken(employee, role);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = request.RefreshToken,
            ExpiresIn = _jwtConfig.AccessTokenExpirationMinutes * 60,
            TokenType = "Bearer",
            Employee = new EmployeeInfoDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Role = role
            }
        };
    }

    /// <summary>
    /// Revoke a refresh token.
    /// </summary>
    public async Task<bool> RevokeRefreshTokenAsync(
        string refreshToken,
        string reason = "User logout")
    {
        if (!RefreshTokenStore.TryGetValue(
                refreshToken,
                out var storedToken))
        {
            return false;
        }

        storedToken.Revoke(reason);

        return true;
    }

    /// <summary>
    /// Validate refresh token status.
    /// </summary>
    public async Task<bool> ValidateRefreshTokenAsync(string token)
    {
        if (!RefreshTokenStore.TryGetValue(token, out var storedToken))
        {
            return false;
        }

        return storedToken.IsValid;
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    /// <summary>
    /// Returns the employee role used for Phase 2.3 RBAC.
    /// </summary>
    /// 
    private static string GetEmployeeRole(Employee employee)
    {
        if (string.IsNullOrEmpty(employee.Role))
        {
            return "Employee";
        }
        return employee.Role;
    }

    /// <summary>
    /// Generate access JWT token.
    /// </summary>
    private string GenerateAccessToken(
        Employee employee,
        string role)
    {
        var key = Encoding.ASCII.GetBytes(_jwtConfig.SecretKey);

        var now = DateTime.UtcNow;

        var expiresAt =
            now.AddMinutes(_jwtConfig.AccessTokenExpirationMinutes);

        var claims = new List<Claim>
        {
            new Claim("sub", employee.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                $"{employee.FirstName} {employee.LastName}"),

            new Claim(
                ClaimTypes.Email,
                employee.Email),

            new Claim(
                ClaimTypes.Role,
                role)
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
    /// Generate a new refresh token.
    /// </summary>
    private RefreshToken GenerateRefreshToken(
        Employee employee,
        string? ipAddress = null,
        string? userAgent = null)
    {
        var randomBytes = new byte[32];

        using var rng =
            System.Security.Cryptography.RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        var token = new RefreshToken(
            employee.Id,
            Convert.ToBase64String(randomBytes))
        {
            IssuedFromIpAddress = ipAddress,
            IssuedFromUserAgent = userAgent
        };

        return token;
    }

    /// <summary>
    /// Verify password against stored hash.
    /// Phase 2.2 temporary plain-string comparison.
    /// </summary>
    private bool VerifyPassword(
        string password,
        string? passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash))
        {
            return false;
        }

        return password == passwordHash;
    }
}