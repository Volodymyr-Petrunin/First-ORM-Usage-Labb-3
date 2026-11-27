using SqlLabbThree.Data;

namespace SqlLabbThree.Services;

public class DepartmentService(SchoolContext context)
{
    public Dictionary<string, int> CountTeachersInDepartment()
    {
        return context.Departments
            .Select(department => new
            {
                DepartmentName = department.Name,
                TeachersCount = department.Employees.Count(employee => employee.ErRoles.Any(role => role.RoleTitle == "Teacher"))
            })
            .ToDictionary(summary => summary .DepartmentName, summary => summary .TeachersCount);
    }
}