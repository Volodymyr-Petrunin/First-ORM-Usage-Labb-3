using System;
using System.Collections.Generic;

namespace SqlLabbThree.Domain;

public partial class Group
{
    public int GroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public int? GroupMentor { get; set; }

    public virtual Employee? GroupMentorNavigation { get; set; }

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
