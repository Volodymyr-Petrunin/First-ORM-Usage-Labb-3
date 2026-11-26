using System.ComponentModel.DataAnnotations.Schema;

namespace SqlLabbThree.Domain;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string CourseDescription { get; set; } = null!;

    [Column("course_start_day")]
    public DateOnly StartDay { get; set; }

    [Column("course_end_day")]
    public DateOnly? EndDay { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Employee> EcTeachers { get; set; } = new List<Employee>();
}
