using System;
using System.Collections.Generic;

namespace SqlLabbThree.Domain;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentName { get; set; } = null!;

    public string StudentSurname { get; set; } = null!;

    public string StudentPersonalNumber { get; set; } = null!;

    public int? StudentGroupId { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual Group? StudentGroup { get; set; }
}
