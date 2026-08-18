using System;
using System.Threading.Tasks;
using FluentAssertions;
using HRMS.Infrastructure.Services;
using Xunit;

namespace HRMS.UnitTests.Services
{
    public class InMemoryAuthorizationServiceTests
    {
        private readonly InMemoryAuthorizationService _authService;

        public InMemoryAuthorizationServiceTests()
        {
            _authService = new InMemoryAuthorizationService();
        }

        [Fact]
        public async Task HasRoleAsync_UserWithRole_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = "HR";
            _authService.AddRoleToUser(userId, role);

            // Act
            var result = await _authService.HasRoleAsync(userId, role);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasRoleAsync_UserWithoutRole_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var role = "Administrator";

            // Act
            var result = await _authService.HasRoleAsync(userId, role);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task HasPermissionAsync_UserWithPermission_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var permission = "Salary.Manage";
            _authService.AddPermissionToUser(userId, permission);

            // Act
            var result = await _authService.HasPermissionAsync(userId, permission);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasPermissionAsync_UserWithoutPermission_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var permission = "Salary.Manage";

            // Act
            var result = await _authService.HasPermissionAsync(userId, permission);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CanAccessEmployeeAsync_OwnProfile_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var targetEmployeeId = userId; // Same user

            // Act
            var result = await _authService.CanAccessEmployeeAsync(userId, targetEmployeeId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CanAccessEmployeeAsync_DirectReportByManager_ReturnsTrue()
        {
            // Arrange
            var managerId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();
            _authService.SetManager(employeeId, managerId);

            // Act
            var result = await _authService.CanAccessEmployeeAsync(managerId, employeeId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CanAccessEmployeeAsync_NotManagerAndNotSelf_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherEmployeeId = Guid.NewGuid();

            // Act
            var result = await _authService.CanAccessEmployeeAsync(userId, otherEmployeeId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CanAccessSalaryDataAsync_HRRole_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var targetEmployeeId = Guid.NewGuid();
            _authService.AddRoleToUser(userId, "HR");
            _authService.AddPermissionToUser(userId, "Salary.Manage");

            // Act
            var result = await _authService.CanAccessSalaryDataAsync(userId, targetEmployeeId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CanAccessSalaryDataAsync_NonHRUser_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var targetEmployeeId = Guid.NewGuid();
            _authService.AddRoleToUser(userId, "Employee");

            // Act
            var result = await _authService.CanAccessSalaryDataAsync(userId, targetEmployeeId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IsAdministratorAsync_AdminRole_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _authService.AddRoleToUser(userId, "Administrator");

            // Act
            var result = await _authService.IsAdministratorAsync(userId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsAdministratorAsync_NonAdminRole_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _authService.AddRoleToUser(userId, "Employee");

            // Act
            var result = await _authService.IsAdministratorAsync(userId);

            // Assert
            result.Should().BeFalse();
        }
    }
}
