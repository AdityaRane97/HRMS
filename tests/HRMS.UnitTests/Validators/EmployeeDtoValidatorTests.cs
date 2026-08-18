using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using HRMS.Application.DTOs;
using HRMS.Application.Validators;
using Xunit;

namespace HRMS.UnitTests.Validators
{
    public class EmployeeDtoValidatorTests
    {
        private readonly IValidator<CreateEmployeeDto> _createValidator;
        private readonly IValidator<UpdateEmployeeDto> _updateValidator;

        public EmployeeDtoValidatorTests()
        {
            _createValidator = new CreateEmployeeDtoValidator();
            _updateValidator = new UpdateEmployeeDtoValidator();
        }

        [Fact]
        public async Task CreateEmployeeDto_ValidData_ShouldPass()
        {
            // Arrange
            var dto = new CreateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                EmployeeCode = "EMP001",
                Department = "Engineering",
                Designation = "Senior Developer",
                JoinDate = DateTime.UtcNow,
                EmploymentType = "FullTime",
                OrganizationId = Guid.NewGuid()
            };

            // Act
            var result = await _createValidator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task CreateEmployeeDto_MissingFirstName_ShouldFail()
        {
            // Arrange
            var dto = new CreateEmployeeDto
            {
                FirstName = string.Empty,
                LastName = "Doe",
                Email = "john.doe@example.com",
                EmployeeCode = "EMP001",
                Department = "Engineering",
                Designation = "Senior Developer",
                JoinDate = DateTime.UtcNow,
                EmploymentType = "FullTime",
                OrganizationId = Guid.NewGuid()
            };

            // Act
            var result = await _createValidator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "FirstName");
        }

        [Fact]
        public async Task CreateEmployeeDto_InvalidEmail_ShouldFail()
        {
            // Arrange
            var dto = new CreateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "invalid-email",
                EmployeeCode = "EMP001",
                Department = "Engineering",
                Designation = "Senior Developer",
                JoinDate = DateTime.UtcNow,
                EmploymentType = "FullTime",
                OrganizationId = Guid.NewGuid()
            };

            // Act
            var result = await _createValidator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Fact]
        public async Task UpdateEmployeeDto_ValidData_ShouldPass()
        {
            // Arrange
            var dto = new UpdateEmployeeDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Designation = "Tech Lead"
            };

            // Act
            var result = await _updateValidator.ValidateAsync(dto);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateEmployeeDto_AllFieldsNull_ShouldPass()
        {
            // Arrange
            var dto = new UpdateEmployeeDto
            {
                FirstName = null,
                LastName = null,
                Designation = null
            };

            // Act
            var result = await _updateValidator.ValidateAsync(dto);

            // Assert
            // Update DTOs typically allow partial updates (all optional)
            result.IsValid.Should().BeTrue();
        }
    }
}
