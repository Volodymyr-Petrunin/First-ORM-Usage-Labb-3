using System;
using System.Collections.Generic;

namespace SqlLabbThree.Domain;

public partial class Grade
{
    public int GradeId { get; set; }

    public int GradeCourse { get; set; }

    public int GradeTeacher { get; set; }

    public int GradeStudent { get; set; }

    public string GradeMark { get; set; } = null!;

    public DateOnly GradeDate { get; set; }

    public virtual Course GradeCourseNavigation { get; set; } = null!;

    public virtual Student GradeStudentNavigation { get; set; } = null!;

    public virtual Employee GradeTeacherNavigation { get; set; } = null!;
}
