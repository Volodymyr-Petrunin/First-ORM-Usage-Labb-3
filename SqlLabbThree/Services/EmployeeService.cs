using Microsoft.EntityFrameworkCore.ChangeTracking;
using SqlLabbThree.Data;
using SqlLabbThree.Domain;

namespace SqlLabbThree.Services;

public class EmployeeService(SchoolContext context)
{
    public Employee? CreateNewEmployee(Employee employee)
    {
        EntityEntry<Employee> entityEntry = context.Employees.Add(employee);
        context.SaveChanges();
        
        return context.Employees.Find(entityEntry.Entity.EmployeeId);
    }
}