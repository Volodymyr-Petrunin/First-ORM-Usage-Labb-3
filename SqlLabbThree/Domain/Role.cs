using System;
using System.Collections.Generic;

namespace SqlLabbThree.Domain;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleTitle { get; set; } = null!;

    public virtual ICollection<Employee> ErEmployees { get; set; } = new List<Employee>();
}
