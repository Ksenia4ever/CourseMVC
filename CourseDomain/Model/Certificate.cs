using System;
using System.Collections.Generic;

namespace CourseDomain.Model;

public partial class Certificate : Entity
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int CourseId { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}
