using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using HRMS.Domain.Entities;
using HRMS.Infrastructure.Services;

namespace HRMS.UnitTests.Services;

/// <summary>
/// Unit tests for InMemoryLeaveService.
/// TODO: User to implement assertions for workflow, balance tracking, notifications
/// </summary>
public class InMemoryLeaveServiceTests
{
    private readonly InMemoryLeaveService _service = new();

    [Fact]
    public async Task SubmitLeaveRequest_WithValidData_ShouldCreateRequest()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var leaveType = "Annual";
        var startDate = DateTime.UtcNow.Date.AddDays(5);
        var endDate = startDate.AddDays(4);
        var reason = "Vacation to Europe for family trip";

        // Act
        var result = await _service.SubmitLeaveRequestAsync(employeeId, leaveType, startDate, endDate, reason);

        // TODO: Assert
        // - result should not be null
        // - result.EmployeeId should equal employeeId
        // - result.LeaveType should equal leaveType
        // - result.RequestStatus should be "Pending"
        // - result.StartDate should equal startDate
        // - result.EndDate should equal endDate

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetLeaveRequestById_WithValidId_ShouldReturnRequest()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var submitted = await _service.SubmitLeaveRequestAsync(
            employeeId, "Sick", DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date.AddDays(2), "Medical emergency"
        );

        // Act
        var result = await _service.GetLeaveRequestByIdAsync(submitted.Id);

        // TODO: Assert
        // - result should not be null
        // - result.Id should equal submitted.Id

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetLeaveRequestsByEmployee_ShouldReturnAllRequestsForEmployee()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var otherEmployeeId = Guid.NewGuid();

        await _service.SubmitLeaveRequestAsync(employeeId, "Annual", DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(9), "Vacation");
        await _service.SubmitLeaveRequestAsync(employeeId, "Sick", DateTime.UtcNow.Date.AddDays(10), DateTime.UtcNow.Date.AddDays(11), "Sick leave");
        await _service.SubmitLeaveRequestAsync(otherEmployeeId, "Annual", DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(7), "Other employee");

        // Act
        var result = await _service.GetLeaveRequestsByEmployeeAsync(employeeId);

        // TODO: Assert
        // - result count should be 2
        // - All results should have EmployeeId == employeeId

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetLeaveRequestsByEmployee_WithStatusFilter_ShouldReturnOnlyMatchingStatus()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        await _service.SubmitLeaveRequestAsync(employeeId, "Annual", DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(9), "Vacation");

        // Act
        var result = await _service.GetLeaveRequestsByEmployeeAsync(employeeId, "Pending");

        // TODO: Assert
        // - result count should be 1
        // - result[0].RequestStatus should be "Pending"

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task ApproveByManager_WithValidData_ShouldUpdateStatus()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var submitted = await _service.SubmitLeaveRequestAsync(
            employeeId, "Annual", DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(9), "Vacation"
        );

        // Act
        var result = await _service.ApproveByManagerAsync(submitted.Id, managerId, "Approved");

        // TODO: Assert
        // - result.RequestStatus should be "ApprovedByManager"
        // - result.ManagerId should equal managerId
        // - result.ManagerApprovedAt should be set
        // - result.ManagerRemarks should equal "Approved"

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task RejectByManager_WithValidData_ShouldUpdateStatusToRejected()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var submitted = await _service.SubmitLeaveRequestAsync(
            employeeId, "Annual", DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(9), "Vacation"
        );

        // Act
        var result = await _service.RejectByManagerAsync(submitted.Id, managerId, "Team has critical project deadline");

        // TODO: Assert
        // - result.RequestStatus should be "RejectedByManager"
        // - result.ManagerId should equal managerId
        // - result.ManagerRemarks should contain rejection reason

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ApproveByHR_WithValidData_ShouldFinalizeApproval()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var hrApproverId = Guid.NewGuid();
        var submitted = await _service.SubmitLeaveRequestAsync(
            employeeId, "Annual", DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(9), "Vacation"
        );
        await _service.ApproveByManagerAsync(submitted.Id, managerId);

        // Act
        var result = await _service.ApproveByHRAsync(submitted.Id, hrApproverId, "Approved and processed");

        // TODO: Assert
        // - result.RequestStatus should be "ApprovedByHR"
        // - result.HRApproverId should equal hrApproverId
        // - result.HRApprovedAt should be set
        // - TODO: Verify leave balance was deducted

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task CancelLeave_WithValidData_ShouldUpdateStatusToCancel()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var hrApproverId = Guid.NewGuid();
        var submitted = await _service.SubmitLeaveRequestAsync(
            employeeId, "Annual", DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(9), "Vacation"
        );
        await _service.ApproveByManagerAsync(submitted.Id, managerId);
        await _service.ApproveByHRAsync(submitted.Id, hrApproverId);

        // Act
        var result = await _service.CancelLeaveAsync(submitted.Id, "Medical treatment required");

        // TODO: Assert
        // - result.RequestStatus should be "Cancelled"
        // - TODO: Verify leave balance was restored

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPendingLeavesForManager_ShouldReturnOnlyPendingRequests()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var employeeId1 = Guid.NewGuid();
        var employeeId2 = Guid.NewGuid();

        await _service.SubmitLeaveRequestAsync(employeeId1, "Annual", DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(7), "Vacation");
        await _service.SubmitLeaveRequestAsync(employeeId2, "Sick", DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date.AddDays(2), "Sick leave");

        // Act
        var result = await _service.GetPendingLeavesForManagerAsync(managerId);

        // TODO: Assert
        // - All results should have RequestStatus == "Pending"
        // - Result should be sorted by start date

        result.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetPendingLeavesForHR_ShouldReturnLeavesAwaitingHRApproval()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var submitted = await _service.SubmitLeaveRequestAsync(
            Guid.NewGuid(), "Annual", DateTime.UtcNow.Date.AddDays(5), DateTime.UtcNow.Date.AddDays(7), "Vacation"
        );
        await _service.ApproveByManagerAsync(submitted.Id, managerId);

        // Act
        var result = await _service.GetPendingLeavesForHRAsync();

        // TODO: Assert
        // - result count should be at least 1
        // - All results should have RequestStatus == "ApprovedByManager"

        result.Should().HaveCountGreaterThanOrEqualTo(1);
    }
}
