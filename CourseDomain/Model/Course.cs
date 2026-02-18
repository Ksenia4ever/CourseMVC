using System;
using System.Collections.Generic;

namespace CourseDomain.Model;

public partial class Course : Entity
{
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int AuthorId { get; set; }

    public string Subject { get; set; } = null!;

    public DateOnly Created { get; set; }

    public DateOnly Modified { get; set; }

    public virtual Account Author { get; set; } = null!;

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual ICollection<CourseAccount> CourseAccounts { get; set; } = new List<CourseAccount>();

    public virtual ICollection<Excercise> Excercises { get; set; } = new List<Excercise>();
}
