using System;
using System.Collections.Generic;

namespace SqlLabbThree.Domain;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string EmployeePersonalNumber { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string EmployeeSurname { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Course> EcCourses { get; set; } = new List<Course>();

    public virtual ICollection<Role> ErRoles { get; set; } = new List<Role>();
    
    public Employee(string employeeName, string employeePersonalNumber, string employeeSurname)
    {
        EmployeeName = employeeName;
        EmployeePersonalNumber = employeePersonalNumber;
        EmployeeSurname = employeeSurname;
    }
    
    public override string ToString()
    {
        return $"Employee: {EmployeeName} {EmployeeSurname}, Personal Number: {EmployeePersonalNumber}";
    }
}
