# ADR-006: Global Error Handling and ProblemDetails

## Status
ACCEPTED

## Context
The HRMS API must provide consistent error responses across all endpoints:
- Developers need standardized error format for integration
- Clients (web, mobile) need actionable error details
- Operators need traceability for debugging and monitoring
- Compliance requires audit trail of error conditions
- API must distinguish between client errors (4xx) and server errors (5xx)

Error scenarios:
- Validation failures (400)
- Authentication failures (401)
- Authorization failures (403)
- Resource not found (404)
- Concurrency conflicts (409)
- Business rule violations (422)
- Unexpected exceptions (500)
- Service unavailable (503)

## Decision
Implement **RFC 7807 ProblemDetails standard** with custom exception hierarchy:

### 1. **Exception Hierarchy** (`HRMS.Application/Exceptions/HrmsException.cs`):
```
HrmsException (base)
  ├── ValidationException
  ├── BusinessRuleViolationException
  ├── ResourceNotFoundException
  ├── UnauthorizedException
  ├── ForbiddenAccessException
  ├── ConcurrencyException
  ├── ExternalServiceException
```

### 2. **Global Error Handling Middleware** (`ErrorHandlingMiddleware.cs`):
```csharp
app.UseErrorHandling();  // Wraps entire request pipeline

// Maps exceptions to ProblemDetails responses:
Exception Type                 → Status Code → Response
ValidationException            → 400
BusinessRuleViolationException → 422
ResourceNotFoundException      → 404
UnauthorizedException        → 401
ForbiddenAccessException      → 403
ConcurrencyException          → 409
ExternalServiceException      → 503
Unhandled Exception           → 500
```

### 3. **ProblemDetails Response Format**:
```json
{
  "type": "https://api.example.com/problems/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Validation failed for the request.",
  "instance": "/employees",
  "traceId": "0HN1GMDL1EA7U:00000001",
  "timestamp": "2026-08-18T12:34:56Z",
  "errors": {
	"FirstName": ["FirstName is required"],
	"Email": ["Invalid email format"]
  }
}
```

### 4. **Logging Strategy**:
- **Info Level**: Validation and resource-not-found errors (client issues)
- **Warn Level**: Authorization and business rule violations (suspicious)
- **Error Level**: Concurrency, external service, unhandled exceptions (operational)
- **Critical Level**: Repeated errors or patterns suggesting attacks

### 5. **Security Considerations**:
- **Hide Internals**: Stack traces excluded from production responses
- **Sanitize Messages**: Business exceptions provide helpful, non-technical hints
- **Correlation IDs**: Every response includes traceId for log correlation
- **No Sensitive Data**: Passwords, tokens, personal info never in error messages

### 6. **Client Implications**:
```csharp
// Client code can now handle errors predictably:
try {
	var response = await client.PostAsJsonAsync("/employees", dto);
	response.EnsureSuccessStatusCode();
} catch (HttpRequestException ex) when (ex.StatusCode == 400) {
	// Handle validation—parse ProblemDetails for field errors
	var problem = JsonSerializer.Deserialize<ProblemDetails>(ex.Content);
	// Display to UI: problem.Extensions["errors"]
}
```

## Rationale
- **RFC 7807 Standard**: Tools and clients expect this format
- **Uniform Interface**: Every error follows the same contract
- **Debuggability**: Correlation IDs link requests to logs
- **Security**: Stack traces hidden from clients
- **Compliance**: Error logging provides audit trail for SOC2

## Consequences
- All exceptions must be mapped to ProblemDetails (overhead upfront)
- Unhandled exceptions log error details (performance hit for failures)
- Clients must be updated to parse new error format
- Custom error types require new exception classes (no generic catch-all)

## Implementation Notes
- Middleware registered **first** in Program.cs to catch all exceptions
- Authorization errors carry permission context (what was required)
- Validation errors include field names and rules that failed
- External service errors include retry-after hints
- Concurrency errors include row version for client-side resolution

## Future Enhancements
- **Rate Limiting Error**: 429 Too Many Requests with retry-after headers
- **Error Codes**: Numeric error codes for client-side localization (`"errorCode": "E001"`)
- **Help Links**: Documentation URI for each error type
- **Problem Store**: Persistent log of errors for analytics
- **Error Notifications**: Alert operators on critical error patterns
