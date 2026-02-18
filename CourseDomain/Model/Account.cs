using System;
using System.Collections.Generic;

namespace CourseDomain.Model;

public partial class Account : Entity
{
    public string Name { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<AccountScourse> AccountScourses { get; set; } = new List<AccountScourse>();

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual ICollection<CourseAccount> CourseAccounts { get; set; } = new List<CourseAccount>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
