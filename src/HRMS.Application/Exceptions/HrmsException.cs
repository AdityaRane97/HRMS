namespace HRMS.Application.Exceptions;

/// <summary>
/// Base exception for all HRMS domain-related errors.
/// </summary>
public class HrmsException : Exception
{
    public string? ErrorCode { get; set; }
    public Dictionary<string, object>? ErrorDetails { get; set; }

    public HrmsException(string message, string? errorCode = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Raised when a business rule is violated.
/// Example: Attempting to approve a leave that's already been approved.
/// </summary>
public class BusinessRuleViolationException : HrmsException
{
    public BusinessRuleViolationException(string message, string? errorCode = null)
        : base(message, errorCode ?? "BUSINESS_RULE_VIOLATION")
    {
    }
}

/// <summary>
/// Raised when a requested resource is not found.
/// </summary>
public class ResourceNotFoundException : HrmsException
{
    public string? ResourceType { get; set; }
    public object? ResourceId { get; set; }

    public ResourceNotFoundException(string resourceType, object resourceId)
        : base($"{resourceType} with ID '{resourceId}' not found.", "RESOURCE_NOT_FOUND")
    {
        ResourceType = resourceType;
        ResourceId = resourceId;
    }

    public ResourceNotFoundException(string message)
        : base(message, "RESOURCE_NOT_FOUND")
    {
    }
}

/// <summary>
/// Raised when a user attempts an operation they're not authorized to perform.
/// </summary>
public class UnauthorizedException : HrmsException
{
    public UnauthorizedException(string message = "User is not authorized to perform this action.")
        : base(message, "UNAUTHORIZED")
    {
    }
}

/// <summary>
/// Raised when access to a resource is denied based on permissions/roles.
/// </summary>
public class ForbiddenAccessException : HrmsException
{
    public string? RequiredPermission { get; set; }
    public string? RequiredRole { get; set; }

    public ForbiddenAccessException(string message = "Access to this resource is forbidden.")
        : base(message, "FORBIDDEN")
    {
    }

    public ForbiddenAccessException(string requiredPermission, string? requiredRole = null)
        : base($"Required permission '{requiredPermission}' not granted.", "FORBIDDEN")
    {
        RequiredPermission = requiredPermission;
        RequiredRole = requiredRole;
    }
}

/// <summary>
/// Raised when a concurrency conflict occurs (optimistic locking failure).
/// </summary>
public class ConcurrencyException : HrmsException
{
    public ConcurrencyException(string message = "The resource was modified by another user. Please refresh and try again.")
        : base(message, "CONCURRENCY_CONFLICT")
    {
    }
}

/// <summary>
/// Raised when validation fails.
/// </summary>
public class ValidationException : HrmsException
{
    public Dictionary<string, string[]> Errors { get; set; } = new();

    public ValidationException(Dictionary<string, string[]> errors)
        : base("Validation failed.", "VALIDATION_ERROR")
    {
        Errors = errors;
    }

    public ValidationException(string message, Dictionary<string, string[]>? errors = null)
        : base(message, "VALIDATION_ERROR")
    {
        if (errors != null)
            Errors = errors;
    }
}

/// <summary>
/// Raised when an external service call fails.
/// </summary>
public class ExternalServiceException : HrmsException
{
    public string? ServiceName { get; set; }
    public int? HttpStatusCode { get; set; }

    public ExternalServiceException(string serviceName, string message, int? httpStatusCode = null, Exception? innerException = null)
        : base($"External service '{serviceName}' error: {message}", "EXTERNAL_SERVICE_ERROR", innerException)
    {
        ServiceName = serviceName;
        HttpStatusCode = httpStatusCode;
    }
}
