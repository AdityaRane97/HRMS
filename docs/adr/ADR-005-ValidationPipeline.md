# ADR-005: Request Validation Pipeline Design

## Status
ACCEPTED

## Context
The HRMS API must validate all incoming requests to:
- Prevent invalid data from entering the domain layer
- Provide clear, consistent error messages to clients
- Enforce business rules early (fail fast)
- Support both data validation (types, formats) and business validation (uniqueness, constraints)

Requirements:
- Declarative validation rules (not scattered in handlers)
- Reusable validators across multiple contexts
- Type-safe validation
- Integration with OpenAPI/Swagger documentation
- Asymmetric validation for create vs. update operations

## Decision
Implement **FluentValidation-based pipeline**:

### 1. **Validation Framework**:
- **NuGet Package**: `FluentValidation` v11.8.0 + `FluentValidation.DependencyInjectionExtensions`
- **Fluent API** for rule definition in `HRMS.Application/Validators/`
- **Auto-registration** via `AddValidatorsFromAssemblies()` in DI

### 2. **Validator Structure**:
```csharp
// Phase 1 validators for DTO contracts
- CreateEmployeeDtoValidator
  Rules: First/LastName required, Email format, EmployeeCode unique

- UpdateEmployeeDtoValidator
  Rules: All fields optional (partial updates), Email format if provided
```

### 3. **Validation Pipeline**:
```
HTTP Request
  ↓
ActionFilter (ValidateModelFilter)
  ↓
Resolve IValidator<T> from DI
  ↓
Run ValidateAsync(request)
  ↓
If valid → Controller Action
If invalid → Return ValidationException
  ↓
Global ErrorHandlingMiddleware
  ↓
Return ProblemDetails (400 Bad Request)
```

### 4. **Error Response Format**:
```json
{
  "type": "about:blank",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Validation failed",
  "errors": {
	"FirstName": ["FirstName is required", "FirstName must not be empty"],
	"Email": ["Invalid email format"]
  },
  "traceId": "0HN1GMDL1EA7U:00000001"
}
```

### 5. **Validator Registration**:
```csharp
// In Program.cs
builder.Services.AddValidatorsFromAssemblies(
	AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers(options =>
{
	options.Filters.Add<ValidateModelFilter>();
});
```

## Rationale
- **Declarative**: Business rules live in validators, not in action methods
- **Reusable**: Same validator can be used across CreateDto, UpdateDto, API tests
- **Composable**: Validators can reference other validators for complex rules
- **Type-Safe**: Generic `IValidator<T>` prevents reflection errors
- **Testable**: Validators are POCOs, easy to unit test in isolation

## Consequences
- FluentValidation introduces a new dependency
- Validation runs before domain logic (can't use database state in Phase 1)
- Async validators (e.g., uniqueness checks) add latency
- Custom rules require custom validators (not generic)

## Implementation Notes
- Validators **only** validate DTOs, not domain entities
- Business uniqueness checks (e.g., EmployeeCode) deferred to domain layer
- Email format validation via regex or `EmailAddress()` rule
- Required field validation uses `NotEmpty()` + `NotNull()` for safety
- Optional fields use conditional validation: `RuleFor(x => x.Email).EmailAddress().When(x => x.Email != null)`

## Future Enhancements
- **Async Validators**: Database uniqueness checks (Phase 2+)
- **Custom Rules**: Domain-specific validators (e.g., ValidEmploymentType)
- **Localization**: Multi-language error messages
- **Swagger Integration**: Validators feed into OpenAPI schema
- **Cross-Field Validation**: Complex multi-property rules
