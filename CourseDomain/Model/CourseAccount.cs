using System;
using System.Collections.Generic;

namespace CourseDomain.Model;

public partial class CourseAccount : Entity
{
    public int CourseId { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
