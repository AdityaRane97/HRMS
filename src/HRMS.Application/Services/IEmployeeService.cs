using HRMS.Domain.Entities;

namespace HRMS.Application.Services;

/// <summary>
/// Employee service contract.
/// Provides employee lookup and management for authentication and authorization.
/// Phase 2.2: Basic username and ID-based lookups for JWT authentication.
/// Phase 3: Add pagination, filtering, bulk operations for full employee management CRUD.
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Get employee by ID.
    /// </summary>
    Task<Employee?> GetEmployeeByIdAsync(Guid employeeId);

    /// <summary>
    /// Get employee by username (for authentication).
    /// Used by IAuthService during login.
    /// </summary>
    Task<Employee?> GetEmployeeByUsernameAsync(string username);

    /// <summary>
    /// Get employee by email.
    /// </summary>
    Task<Employee?> GetEmployeeByEmailAsync(string email);

    /// <summary>
    /// Check if employee with given username exists.
    /// </summary>
    Task<bool> EmployeeExistsByUsernameAsync(string username);
}
