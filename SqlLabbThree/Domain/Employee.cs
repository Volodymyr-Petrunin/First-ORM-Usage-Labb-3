using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SqlLabbThree.Domain;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string EmployeePersonalNumber { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string EmployeeSurname { get; set; } = null!;

    [Column("employee_salary")]
    [Precision(10, 2)]
    public decimal Salary { get; set; }

    [Column("employee_start_day")]
    public DateOnly StartDay { get; set; }
    
    [Column("employee_end_day")]
    public DateOnly? EndDay { get; set; }
    
    [Column("department_id")]
    public int DepartmentId { get; set; }

    [DeleteBehavior(DeleteBehavior.Restrict)]
    public Department Department { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Course> EcCourses { get; set; } = new List<Course>();

    public virtual ICollection<Role> ErRoles { get; set; } = new List<Role>();
    
    public Employee(string employeeName, string employeeSurname, string employeePersonalNumber)
    {
        EmployeeName = employeeName;
        EmployeeSurname = employeeSurname;
        EmployeePersonalNumber = employeePersonalNumber;
    }
    
    public override string ToString()
    {
        return $"Employee: ID: {EmployeeId} Full Name: {EmployeeName} {EmployeeSurname}, Personal Number: {EmployeePersonalNumber}";
    }
}
