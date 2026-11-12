using System;
using System.Collections.Generic;

namespace SqlLabbThree.Domain;

public partial class Lesson
{
    public int LessonId { get; set; }

    public string LessonName { get; set; } = null!;

    public string LessonAudience { get; set; } = null!;

    public string LessonDayOfWeek { get; set; } = null!;

    public TimeOnly LessonStartTime { get; set; }

    public TimeOnly LessonEndTime { get; set; }

    public int LessonTeacherId { get; set; }

    public int LessonCourseId { get; set; }

    public int LessonGroupId { get; set; }

    public virtual Course LessonCourse { get; set; } = null!;

    public virtual Group LessonGroup { get; set; } = null!;

    public virtual Employee LessonTeacher { get; set; } = null!;
}
