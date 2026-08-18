using HRMS.Application.DTOs;
using HRMS.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Api.Controllers;

/// <summary>
/// Authentication controller.
/// Phase 2.2: Login (POST /auth/login) and Refresh (POST /auth/refresh) endpoints.
/// No authorization required (public endpoints for unauthenticated clients).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Authenticate user with username and password.
    /// Returns access token (15 min) and refresh token (7 days).
    /// Phase 2.2: No encryption in transit (use HTTPS in production).
    /// Captures IP address and User-Agent for audit trail.
    /// </summary>
    /// <response code="200">Successfully authenticated. Returns TokenResponseDto with access and refresh tokens.</response>
    /// <response code="400">Invalid request. Username or password missing.</response>
    /// <response code="401">Authentication failed. Invalid credentials.</response>
    /// <response code="500">Internal server error during authentication.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (request == null)
                return BadRequest(new { message = "Request body is required" });

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Username and password are required" });

            // Capture IP and User-Agent for audit
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            request.IpAddress = ipAddress;
            request.UserAgent = userAgent;

            // Attempt login
            var response = await _authService.LoginAsync(request);

            if (response == null)
            {
                _logger.LogWarning($"Failed login attempt for username: {request.Username} from IP: {ipAddress}");
                return Unauthorized(new { message = "Invalid username or password" });
            }

            _logger.LogInformation($"Successful login for employee: {response.Employee?.Id} from IP: {ipAddress}");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { message = "Internal server error during authentication" });
        }
    }

    /// <summary>
    /// Refresh access token using a valid refresh token.
    /// Returns new access token (15 min) and same refresh token.
    /// Phase 2.2: Refresh tokens do not rotate by default (can implement rotation in Phase 3).
    /// Validates refresh token expiration and revocation status.
    /// </summary>
    /// <response code="200">Successfully refreshed. Returns new TokenResponseDto with new access token.</response>
    /// <response code="400">Invalid request. Refresh token missing.</response>
    /// <response code="401">Refresh token invalid, expired, or revoked.</response>
    /// <response code="500">Internal server error during token refresh.</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            if (request == null)
                return BadRequest(new { message = "Request body is required" });

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            // Validate and refresh
            var response = await _authService.RefreshTokenAsync(request);

            if (response == null)
            {
                _logger.LogWarning("Failed token refresh: invalid or expired refresh token");
                return Unauthorized(new { message = "Invalid, expired, or revoked refresh token" });
            }

            _logger.LogInformation($"Successful token refresh for employee: {response.Employee?.Id}");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Internal server error during token refresh" });
        }
    }

    /// <summary>
    /// Logout by revoking the current refresh token.
    /// Phase 2.2: Simple logout; refresh token becomes invalid immediately.
    /// Phase 3: Add to blacklist/audit table for persistent revocation.
    /// </summary>
    /// <response code="200">Successfully logged out. Refresh token revoked.</response>
    /// <response code="400">Invalid request. Refresh token missing.</response>
    /// <response code="500">Internal server error during logout.</response>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            var revoked = await _authService.RevokeRefreshTokenAsync(request.RefreshToken, "User logout");

            if (!revoked)
                return BadRequest(new { message = "Refresh token not found or already revoked" });

            _logger.LogInformation("Successful logout");
            return Ok(new { message = "Successfully logged out" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Internal server error during logout" });
        }
    }
}
