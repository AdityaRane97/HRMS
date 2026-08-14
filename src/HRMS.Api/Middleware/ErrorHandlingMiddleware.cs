using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using HRMS.Application.Exceptions;
using System.Text.Json;

namespace HRMS.Api.Middleware;

/// <summary>
/// Global exception handling middleware.
/// Catches all unhandled exceptions and returns consistent ProblemDetails responses.
/// Ensures sensitive information is not exposed in error responses.
/// Supports correlation IDs for request tracking.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var correlationId = context.TraceIdentifier ?? Guid.NewGuid().ToString();

        var (statusCode, problemDetails) = GetProblemDetails(exception, correlationId);
        context.Response.StatusCode = statusCode;

        // Log the exception
        LogException(exception, statusCode, correlationId);

        return context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static (int statusCode, ProblemDetails details) GetProblemDetails(Exception exception, string correlationId)
    {
        return exception switch
        {
            ValidationException ve =>
                (StatusCodes.Status400BadRequest,
                    new ProblemDetails
                    {
                        Type = "https://hrms.example.com/errors/validation",
                        Title = "Validation Failed",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = exception.Message,
                        Instance = correlationId,
                        Extensions = new Dictionary<string, object?>
                        {
                            { "traceId", correlationId },
                            { "errorCode", ve.ErrorCode },
                            { "validationErrors", ve.Errors }
                        }
                    }),

            UnauthorizedException =>
                (StatusCodes.Status401Unauthorized,
                    new ProblemDetails
                    {
                        Type = "https://hrms.example.com/errors/unauthorized",
                        Title = "Unauthorized",
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = exception.Message,
                        Instance = correlationId,
                        Extensions = new Dictionary<string, object?>
                        {
                            { "traceId", correlationId }
                        }
                    }),

            ForbiddenAccessException fae =>
                (StatusCodes.Status403Forbidden,
                    new ProblemDetails
                    {
                        Type = "https://hrms.example.com/errors/forbidden",
                        Title = "Forbidden",
                        Status = StatusCodes.Status403Forbidden,
                        Detail = exception.Message,
                        Instance = correlationId,
                        Extensions = new Dictionary<string, object?>
                        {
                            { "traceId", correlationId },
                            { "requiredPermission", fae.RequiredPermission }
                        }
                    }),

            ResourceNotFoundException rnfe =>
                (StatusCodes.Status404NotFound,
                    new ProblemDetails
                    {
                        Type = "https://hrms.example.com/errors/notfound",
                        Title = "Resource Not Found",
                        Status = StatusCodes.Status404NotFound,
                        Detail = exception.Message,
                        Instance = correlationId,
                        Extensions = new Dictionary<string, object?>
                        {
                            { "traceId", correlationId },
                            { "resourceType", rnfe.ResourceType },
                            { "resourceId", rnfe.ResourceId }
                        }
                    }),

            ConcurrencyException =>
                (StatusCodes.Status409Conflict,
                    new ProblemDetails
                    {
                        Type = "https://hrms.example.com/errors/concurrency",
                        Title = "Concurrency Conflict",
                        Status = StatusCodes.Status409Conflict,
                        Detail = exception.Message,
                        Instance = correlationId,
                        Extensions = new Dictionary<string, object?>
                        {
                            { "traceId", correlationId }
                        }
                    }),

            BusinessRuleViolationException bre =>
                (StatusCodes.Status422UnprocessableEntity,
                    new ProblemDetails
                    {
                        Type = "https://hrms.example.com/errors/businessrule",
                        Title = "Business Rule Violation",
                        Status = StatusCodes.Status422UnprocessableEntity,
                        Detail = exception.Message,
                        Instance = correlationId,
                        Extensions = new Dictionary<string, object?>
                        {
                            { "traceId", correlationId },
                            { "errorCode", bre.ErrorCode }
                        }
                    }),

            ExternalServiceException ese =>
                (StatusCodes.Status502BadGateway,
                    new ProblemDetails
                    {
                        Type = "https://hrms.example.com/errors/externalservice",
                        Title = "External Service Error",
                        Status = StatusCodes.Status502BadGateway,
                        Detail = exception.Message,
                        Instance = correlationId,
                        Extensions = new Dictionary<string, object?>
                        {
                            { "traceId", correlationId },
                            { "serviceName", ese.ServiceName }
                        }
                    }),

            _ =>
                (StatusCodes.Status500InternalServerError,
                    new ProblemDetails
                    {
                        Type = "https://hrms.example.com/errors/internal",
                        Title = "Internal Server Error",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = "An unexpected error occurred. Please contact support.",
                        Instance = correlationId,
                        Extensions = new Dictionary<string, object?>
                        {
                            { "traceId", correlationId }
                        }
                    })
        };
    }

    private static void LogException(Exception exception, int statusCode, string correlationId)
    {
        if (statusCode >= 500)
        {
            Log.Error(exception, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);
        }
        else if (statusCode >= 400)
        {
            Log.Warning(exception, "Client error. CorrelationId: {CorrelationId}", correlationId);
        }
        else
        {
            Log.Information("Request processed with status {StatusCode}. CorrelationId: {CorrelationId}", statusCode, correlationId);
        }
    }
}

/// <summary>
/// Extension method to register error handling middleware.
/// </summary>
public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}
