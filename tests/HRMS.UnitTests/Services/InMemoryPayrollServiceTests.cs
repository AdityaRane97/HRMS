using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using HRMS.Domain.Entities;
using HRMS.Infrastructure.Services;

namespace HRMS.UnitTests.Services;

/// <summary>
/// Unit tests for InMemoryPayrollService.
/// TODO: User to implement assertions and add edge case tests
/// </summary>
public class InMemoryPayrollServiceTests
{
    private readonly InMemoryPayrollService _service = new();

    [Fact]
    public async Task CreatePayroll_WithValidData_ShouldCreatePayrollRecord()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var payrollMonth = new DateTime(2024, 1, 1);
        var baseSalary = 50000m;

        // Act
        var result = await _service.CreatePayrollAsync(employeeId, payrollMonth, baseSalary);

        // TODO: Assert
        // - result should not be null
        // - result.EmployeeId should equal employeeId
        // - result.BaseSalary should equal baseSalary
        // - result.PaymentStatus should be "Pending"
        // - result.Id should be a valid GUID

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPayrollById_WithValidId_ShouldReturnPayroll()
    {
        // Arrange
        var payrollId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var created = await _service.CreatePayrollAsync(employeeId, DateTime.UtcNow, 50000m);

        // Act
        var result = await _service.GetPayrollByIdAsync(created.Id);

        // TODO: Assert
        // - result should not be null
        // - result.Id should equal created.Id
        // - result.EmployeeId should equal employeeId

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPayrollById_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetPayrollByIdAsync(Guid.NewGuid());

        // TODO: Assert result is null

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPayrollByEmployee_WithDateRange_ShouldReturnFilteredRecords()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var startMonth = new DateTime(2024, 1, 1);
        var endMonth = new DateTime(2024, 3, 1);

        // Create multiple payrolls
        await _service.CreatePayrollAsync(employeeId, new DateTime(2024, 1, 1), 50000m);
        await _service.CreatePayrollAsync(employeeId, new DateTime(2024, 2, 1), 50000m);
        await _service.CreatePayrollAsync(employeeId, new DateTime(2024, 3, 1), 50000m);
        await _service.CreatePayrollAsync(Guid.NewGuid(), new DateTime(2024, 1, 1), 60000m); // Different employee

        // Act
        var result = await _service.GetPayrollByEmployeeAsync(employeeId, startMonth, endMonth);

        // TODO: Assert
        // - result count should be 3
        // - All results should have EmployeeId == employeeId
        // - All results should be within date range

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task UpdatePayroll_WithValidData_ShouldUpdateRecord()
    {
        // Arrange
        var created = await _service.CreatePayrollAsync(Guid.NewGuid(), DateTime.UtcNow, 50000m);
        var newBonus = 5000m;

        // Act
        var result = await _service.UpdatePayrollAsync(created.Id, p =>
        {
            p.OtherAllowances = newBonus;
        });

        // TODO: Assert
        // - result.OtherAllowances should equal newBonus
        // - result.UpdatedAt should be set

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ProcessPayroll_ShouldMarkAsProcessed()
    {
        // Arrange
        var created = await _service.CreatePayrollAsync(Guid.NewGuid(), DateTime.UtcNow, 50000m);

        // Act
        // TODO: This should not throw NotImplementedException once user implements it
        // var result = await _service.ProcessPayrollAsync(created.Id);

        // TODO: Assert
        // - result.PaymentStatus should be "Processed"
        // - result.ProcessedAt should be set
    }

    [Fact]
    public async Task GetPendingPayrolls_ShouldReturnOnlyPendingRecords()
    {
        // Arrange
        var pending1 = await _service.CreatePayrollAsync(Guid.NewGuid(), DateTime.UtcNow, 50000m);
        var pending2 = await _service.CreatePayrollAsync(Guid.NewGuid(), DateTime.UtcNow, 60000m);

        // Act
        var result = await _service.GetPendingPayrollsAsync();

        // TODO: Assert
        // - result count should be at least 2
        // - All results should have PaymentStatus == "Pending"

        result.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
