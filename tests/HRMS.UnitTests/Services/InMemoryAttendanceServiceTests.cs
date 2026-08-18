using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using HRMS.Domain.Entities;
using HRMS.Infrastructure.Services;

namespace HRMS.UnitTests.Services;

/// <summary>
/// Unit tests for InMemoryAttendanceService.
/// TODO: User to implement assertions for check-in/out, approval, and summary logic
/// </summary>
public class InMemoryAttendanceServiceTests
{
    private readonly InMemoryAttendanceService _service = new();

    [Fact]
    public async Task CheckIn_WithValidData_ShouldCreateAttendanceLog()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var checkInTime = DateTime.UtcNow;
        var location = "Office";

        // Act
        var result = await _service.CheckInAsync(employeeId, checkInTime, location);

        // TODO: Assert
        // - result should not be null
        // - result.EmployeeId should equal employeeId
        // - result.CheckInTime should equal checkInTime
        // - result.AttendanceStatus should be "Present"
        // - result.Location should equal location

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckOut_WithValidData_ShouldRecordCheckOutAndCalculateHours()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var checkInTime = DateTime.UtcNow.AddHours(-8); // 8 hours ago
        var checkOutTime = DateTime.UtcNow;

        await _service.CheckInAsync(employeeId, checkInTime, "Office");

        // Act
        var result = await _service.CheckOutAsync(employeeId, checkOutTime);

        // TODO: Assert
        // - result.CheckOutTime should equal checkOutTime
        // - result.WorkedHours should be calculated (approximately 8 hours minus break)
        // - result.AttendanceStatus should still be "Present"

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CheckOut_WithoutCheckIn_ShouldThrowException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var checkOutTime = DateTime.UtcNow;

        // Act & Assert
        // TODO: Assert that Func throws KeyNotFoundException

        Func<Task> action = async () =>
        {
            await _service.CheckOutAsync(employeeId, checkOutTime);
        };

        await Assert.ThrowsAsync<KeyNotFoundException>(action);
    }

    [Fact]
    public async Task GetAttendanceByDate_ShouldReturnLogForThatDate()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date;
        var checkInTime = new DateTime(date.Year, date.Month, date.Day, 9, 0, 0);

        await _service.CheckInAsync(employeeId, checkInTime, "Office");

        // Act
        var result = await _service.GetAttendanceByDateAsync(employeeId, date);

        // TODO: Assert
        // - result should not be null
        // - result.AttendanceDate should equal date
        // - result.CheckInTime should be set

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAttendanceByRange_ShouldReturnAllLogsInRange()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.Date.AddDays(-5);
        var endDate = DateTime.UtcNow.Date;

        for (int i = 0; i < 6; i++)
        {
            var date = DateTime.UtcNow.Date.AddDays(-5 + i);
            await _service.CheckInAsync(employeeId, date.AddHours(9), "Office");
        }

        // Act
        var result = await _service.GetAttendanceByRangeAsync(employeeId, startDate, endDate);

        // TODO: Assert
        // - result count should be 6
        // - All results should have EmployeeId == employeeId
        // - All dates should be within range
        // - Results should be ordered by date

        result.Should().HaveCount(6);
    }

    [Fact]
    public async Task ApproveAttendance_WithValidData_ShouldUpdateStatus()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var approverId = Guid.NewGuid();
        var date = DateTime.UtcNow.Date;
        var status = "LeaveApproved";

        await _service.CheckInAsync(employeeId, date.AddHours(9), "Office");

        // Act
        var result = await _service.ApproveAttendanceAsync(employeeId, date, status, approverId, "Approved for jury duty");

        // TODO: Assert
        // - result.AttendanceStatus should equal status
        // - result.ApprovedBy should equal approverId
        // - result.ApprovedAt should be set

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAttendanceSummary_ShouldReturnAggregatedStats()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.Date.AddDays(-4);
        var endDate = DateTime.UtcNow.Date;

        // Create 5 days of attendance
        for (int i = 0; i < 5; i++)
        {
            var date = startDate.AddDays(i);
            await _service.CheckInAsync(employeeId, date.AddHours(9), "Office");
            await _service.CheckOutAsync(employeeId, date.AddHours(17));
        }

        // Act
        var result = await _service.GetAttendanceSummaryAsync(employeeId, startDate, endDate);

        // TODO: Assert
        // - result.EmployeeId should equal employeeId
        // - result.PresentDays should be 5
        // - result.TotalWorkedHours should be approximately 40 (8 hours/day - break)
        // - result.AverageWorkedHours should be approximately 8

        result.Should().NotBeNull();
    }
}
