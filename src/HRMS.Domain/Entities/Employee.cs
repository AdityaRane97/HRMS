namespace HRMS.Domain.Entities;

public class Employee
{
    public int EmployeeId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public int? ReportingManagerId { get; set; }
}
