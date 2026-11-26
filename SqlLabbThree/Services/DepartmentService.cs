using Microsoft.EntityFrameworkCore;
using SqlLabbThree.Data;
using SqlLabbThree.Domain;

namespace SqlLabbThree.Services;

public class DepartmentService(SchoolContext context)
{
    public Dictionary<string, int> CountTeachersInDepartment()
    {
        Dictionary<string, int> teachersInDepartment = new Dictionary<string, int>();
        
        List<Department> departments = context.Departments
            .Include(department => department.Employees)
            .ThenInclude(employee => employee.ErRoles)
            .ToList();

        foreach (Department department in departments)
        {
            int teachersCount = department.Employees
                .Count(employee => employee.ErRoles.Any(role => role.RoleTitle == "Teacher"));
            
            teachersInDepartment[department.Name] = teachersCount;
        }
        
        return teachersInDepartment;
    }
}