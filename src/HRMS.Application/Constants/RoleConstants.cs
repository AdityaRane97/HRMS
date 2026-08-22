namespace HRMS.Application.Constants;

public static class RoleConstants
{
    public const string Employee = "Employee";

    public const string Manager = "Manager";

    public const string HR = "HR";

    public const string Admin = "Admin";

    public const string HROrAdmin = HR + "," + Admin;

    public const string ManagerHROrAdmin = Manager + "," + HR + "," + Admin;
}