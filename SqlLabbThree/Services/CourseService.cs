using SqlLabbThree.Data;
using SqlLabbThree.Domain;

namespace SqlLabbThree.Services;

public class CourseService(SchoolContext context)
{
    public List<Course> GetAllActiveCourses()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        return context.Courses.Where(c => c.StartDay <= today && (c.EndDay == null || c.EndDay > today)).ToList();
    }
}