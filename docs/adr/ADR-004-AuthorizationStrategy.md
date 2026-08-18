# ADR-004: Authorization and RBAC Implementation

## Status
ACCEPTED

## Context
The HRMS application requires multi-layered authorization:
- **Role-Based Access Control (RBAC)**: HR admins, managers, employees, contractors
- **Hierarchy-Based Access**: Managers can view their direct reports
- **Salary Data Protection**: Stricter rules for sensitive payroll information
- **Organization Scoping**: Eventually, support for multi-tenant data isolation
- **Admin Override**: System administrators bypass most checks

Decisions must balance:
- Simplicity and testability (Phase 1)
- Security enforcement (prevent privilege escalation)
- Performance (avoid N+1 queries on access checks)
- Flexibility (support future client-specific rules)

## Decision
Implement **tiered authorization with delegation pattern**:

### 1. **Core Authorization Service** (`IAuthorizationService`):
```
Public Methods:
  - HasRoleAsync(userId, roleName) → bool
  - HasPermissionAsync(userId, permissionName) → bool
  - CanAccessEmployeeAsync(actor, target) → bool (hierarchy-aware)
  - GetAccessibleEmployeeIdsAsync(userId) → IEnumerable<Guid> (pre-filter)
  - CanAccessSalaryDataAsync(actor, target) → bool (stricter)
  - IsAdministratorAsync(userId) → bool
  - GetUserRolesAsync(userId) → IEnumerable<string>
  - GetUserPermissionsAsync(userId) → IEnumerable<string>
```

### 2. **Authorization Layers**:
- **Layer 1 - Authentication**: JWT token validation (Phase 2)
- **Layer 2 - RBAC**: Role and permission checks via `IAuthorizationService`
- **Layer 3 - Hierarchy**: Manager relationship checks
- **Layer 4 - Resource**: Organization/department scoping (Phase 2+)

### 3. **Phase 1 Implementation** (`InMemoryAuthorizationService`):
- In-memory dictionary-backed role/permission/hierarchy storage
- Test helpers: `AddRoleToUser()`, `AddPermissionToUser()`, `SetManager()`
- No database dependency—purely for Phase 1 testing

### 4. **Phase 2+ Database Implementation**:
- Roles and permissions stored in `Role` and `Permission` entities
- User-role mappings in `UserRole` join table
- Manager relationships via `Employee.ManagerId`
- Cache layer for frequently-accessed permissions

### 5. **Access Control Logic**:
**Employee Access:**
```
if (actor is Administrator) → allow all
if (actor is HR) → allow all employees
if (actor == target) → allow self access
if (actor is manager of target) → allow direct reports
else → deny
```

**Salary Access (stricter):**
```
if (actor has Salary.Manage permission) → allow all
if (actor == target && actor has Salary.ReadOwn) → allow self
if (actor is manager of target && has Salary.ReadTeam) → allow team
else → deny
```

## Rationale
- **Defense in Depth**: Multiple authorization layers prevent single-point failures
- **Testability**: In-memory Phase 1 enables unit testing without database
- **Scalability**: Pre-filtering with `GetAccessibleEmployeeIdsAsync()` avoids client-side filtering
- **Flexibility**: Hierarchy checks decouple from RBAC (support both together)
- **Compliance**: Explicit "deny by default" prevents accidental access grants

## Consequences
- Authorization checks run on every protected request (mitigated by caching)
- Hierarchy traversal could be expensive for large teams (mitigated with indices)
- Client must call `GetAccessibleEmployeeIdsAsync()` to pre-filter queries
- Custom rules require new methods (no generic policy DSL)

## Implementation Notes
- No cookies/sessions—JWT tokens carry minimal claims (userId, roles)
- Authorization service resolves roles/permissions at request time (not cached in token)
- Manager hierarchy leverages EF Core navigation for efficient traversal
- All exceptions converted to `ForbiddenAccessException` with permission context

## Future Enhancements
- **Attribute-Based Access Control (ABAC)**: Support dynamic policies (Phase 3+)
- **Fine-Grained Permissions**: Sub-resource permissions (e.g., `Salary.Approve.Department`)
- **Audit Logging**: Track all authorization checks for compliance
- **Redis Caching**: Cache roles/permissions with TTL for performance
- **Multi-Tenancy**: Organization-scoped access rules
