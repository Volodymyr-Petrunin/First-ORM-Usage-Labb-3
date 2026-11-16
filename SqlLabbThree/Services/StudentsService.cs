using Microsoft.EntityFrameworkCore;
using SqlLabbThree.Data;
using SqlLabbThree.Domain;
using SqlLabbThree.Services.Direction;

namespace SqlLabbThree.Services;

public class StudentsService(SchoolContext context)
{
    public List<Student> GetAllStudents(StudentSortField studentSortField, SortDirection sortDirection)
    {
        IQueryable<Student> query = context.Students.Include(student => student.StudentGroup);

        query = (studentSortField, sortDirection) switch
        {
            (StudentSortField.FirstName, SortDirection.Ascending) => 
                query.OrderBy(student => student.StudentName).ThenBy(student => student.StudentSurname),
            (StudentSortField.FirstName, SortDirection.Descending) =>
                query.OrderByDescending(student => student.StudentName).ThenByDescending(student => student.StudentSurname),
            (StudentSortField.LastName, SortDirection.Ascending) => 
                query.OrderBy(student => student.StudentSurname).ThenBy(student => student.StudentName),
            (StudentSortField.LastName, SortDirection.Descending) =>
                query.OrderByDescending(student => student.StudentSurname).ThenByDescending(student => student.StudentName),
            _ => throw new ArgumentOutOfRangeException(nameof(studentSortField)),
        };
        
        return query.ToList();
    }
}