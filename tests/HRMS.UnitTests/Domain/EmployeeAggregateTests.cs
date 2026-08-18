using System;
using FluentAssertions;
using HRMS.Domain.Entities;
using Xunit;

namespace HRMS.UnitTests.Domain
{
    public class EmployeeAggregateTests
    {
        [Fact]
        public void GetFullName_ReturnsCorrectFormat()
        {
            // Arrange
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                EmployeeCode = "EMP001"
            };

            // Act
            var fullName = employee.GetFullName();

            // Assert
            fullName.Should().Be("John Doe");
        }

        [Fact]
        public void IsCurrentlyEmployed_ActiveStatus_ReturnsTrue()
        {
            // Arrange
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                EmployeeCode = "EMP002",
                EmploymentStatus = "Active"
            };

            // Act
            var isEmployed = employee.IsCurrentlyEmployed();

            // Assert
            isEmployed.Should().BeTrue();
        }

        [Fact]
        public void IsCurrentlyEmployed_InactiveStatus_ReturnsFalse()
        {
            // Arrange
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Johnson",
                Email = "bob.johnson@example.com",
                EmployeeCode = "EMP003",
                EmploymentStatus = "Resigned"
            };

            // Act
            var isEmployed = employee.IsCurrentlyEmployed();

            // Assert
            isEmployed.Should().BeFalse();
        }

        [Fact]
        public void SetManager_SetsManagerAndRelationship()
        {
            // Arrange
            var manager = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Manager",
                Email = "alice@example.com",
                EmployeeCode = "MGR001"
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Employee",
                Email = "bob@example.com",
                EmployeeCode = "EMP004"
            };

            // Act
            employee.SetManager(manager);

            // Assert
            employee.ManagerId.Should().Be(manager.Id);
            employee.Manager.Should().Be(manager);
        }

        [Fact]
        public void SetManager_NullManager_ClearsManagerRelationship()
        {
            // Arrange
            var manager = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Manager",
                Email = "alice@example.com",
                EmployeeCode = "MGR001"
            };

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Employee",
                Email = "bob@example.com",
                EmployeeCode = "EMP004"
            };

            employee.SetManager(manager);

            // Act
            employee.SetManager(null);

            // Assert
            employee.ManagerId.Should().BeNull();
            employee.Manager.Should().BeNull();
        }
    }
}
