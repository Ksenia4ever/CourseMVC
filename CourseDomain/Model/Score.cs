using System;
using System.Collections.Generic;

namespace CourseDomain.Model;

public partial class Score : Entity
{
    public int ExcersiceId { get; set; }

    public int Value { get; set; }

    public virtual ICollection<AccountScourse> AccountScourses { get; set; } = new List<AccountScourse>();

    public virtual Excercise Excersice { get; set; } = null!;
}
