using Microsoft.EntityFrameworkCore;
using SqlLabbThree.Data;
using SqlLabbThree.Domain;

namespace SqlLabbThree.Services;

public class StudentsService(SchoolContext context)
{
    public List<Student> GetAllStudents()
    {
        return context.Students.Include<Student, Group>(student => student.StudentGroup).ToList();
    }
}