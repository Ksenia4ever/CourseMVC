using System;
using System.Collections.Generic;

namespace CourseDomain.Model;

public partial class Excercise : Entity
{
    public string Title { get; set; } = null!;

    public string TaskDescription { get; set; } = null!;

    public string Questions { get; set; } = null!;

    public string AnswerVarients { get; set; } = null!;

    public string Answer { get; set; } = null!;

    public int CourseId { get; set; }

    public DateOnly Created { get; set; }

    public DateOnly Modified { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Score? Score { get; set; }
}
