using HRMS.Application.Services;
using HRMS.Domain.Entities;

namespace HRMS.Infrastructure.Services;

/// <summary>
/// In-memory employee service.
/// Phase 2.2: Stores employees in static dictionary for testing/demo.
/// Phase 3: Replace with EF Core DbSet queries from HrmsDbContext.
/// </summary>
public class InMemoryEmployeeService : IEmployeeService
{
    // Phase 2.2: In-memory employee store (for demo/testing)
    // Phase 3: Replace with DbSet<Employee> from HrmsDbContext
    private static readonly Dictionary<Guid, Employee> EmployeeStore = new();

    public InMemoryEmployeeService()
    {
        // Initialize with demo employees for Phase 2.2 testing
        InitializeDemoEmployees();
    }

    /// <summary>
    /// Get employee by ID.
    /// </summary>
    public async Task<Employee?> GetEmployeeByIdAsync(Guid employeeId)
    {
        return await Task.FromResult(
            EmployeeStore.TryGetValue(employeeId, out var employee) ? employee : null
        );
    }

    /// <summary>
    /// Get employee by username (for authentication).
    /// </summary>
    public async Task<Employee?> GetEmployeeByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return null;

        var employee = EmployeeStore.Values.FirstOrDefault(e =>
            e.Username != null && e.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
        );

        return await Task.FromResult(employee);
    }

    /// <summary>
    /// Get employee by email.
    /// </summary>
    public async Task<Employee?> GetEmployeeByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var employee = EmployeeStore.Values.FirstOrDefault(e =>
            e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
        );

        return await Task.FromResult(employee);
    }

    /// <summary>
    /// Check if employee with given username exists.
    /// </summary>
    public async Task<bool> EmployeeExistsByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return false;

        var exists = EmployeeStore.Values.Any(e =>
            e.Username != null && e.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
        );

        return await Task.FromResult(exists);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    /// <summary>
    /// Initialize demo employees for Phase 2.2 testing.
    /// Phase 3: Remove this; employees will come from database.
    /// Passwords: plain text for demo only! Use BCrypt hashing in production.
    /// </summary>
    private void InitializeDemoEmployees()
    {
        // Only add demo data if store is already initialized
        if (EmployeeStore.Count > 0)
            return;

        var orgId = Guid.NewGuid();

        // ============================================================
        // HR EMPLOYEE
        // ============================================================
        var hrEmployee = new Employee(
            "John",
            "HR Admin",
            "john.hr@company.com",
            "HR001",
            DateTime.Now.AddYears(-5),
            orgId)
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Username = "john.hr",
            PasswordHash = "demo_password_123",
            EmploymentStatus = "Active",
            Role = "HR"
        };

        // ============================================================
        // MANAGER
        // ============================================================
        var managerEmployee = new Employee(
            "Mike",
            "Manager",
            "mike.manager@company.com",
            "MGR001",
            DateTime.Now.AddYears(-4),
            orgId)
        {
            Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
            Username = "mike.manager",
            PasswordHash = "demo_password_789",
            EmploymentStatus = "Active",
            Role = "Manager"
        };

        // ============================================================
        // REGULAR EMPLOYEE
        // ============================================================
        var regularEmployee = new Employee(
            "Jane",
            "Developer",
            "jane.dev@company.com",
            "EMP001",
            DateTime.Now.AddYears(-2),
            orgId)
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000001"),
            Username = "jane.dev",
            PasswordHash = "demo_password_456",
            EmploymentStatus = "Active",
            Role = "Employee",

            // Jane reports to Mike
            ManagerId = managerEmployee.Id
        };

        // ============================================================
        // ADMIN
        // ============================================================
        var adminEmployee = new Employee(
            "Admin",
            "User",
            "admin@company.com",
            "ADM001",
            DateTime.Now.AddYears(-6),
            orgId)
        {
            Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
            Username = "admin",
            PasswordHash = "demo_password_000",
            EmploymentStatus = "Active",
            Role = "Admin"
        };

        // ============================================================
        // ADD ALL DEMO EMPLOYEES TO STORE
        // ============================================================
        EmployeeStore[hrEmployee.Id] = hrEmployee;
        EmployeeStore[managerEmployee.Id] = managerEmployee;
        EmployeeStore[regularEmployee.Id] = regularEmployee;
        EmployeeStore[adminEmployee.Id] = adminEmployee;
    }
}
