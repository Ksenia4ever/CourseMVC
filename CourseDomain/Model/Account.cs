using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseDomain.Model;

public partial class Account : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Ім'я")]
    public string Name { get; set; } = null!;

    public virtual ICollection<AccountScourse> AccountScourses { get; set; } = new List<AccountScourse>();

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual ICollection<CourseAccount> CourseAccounts { get; set; } = new List<CourseAccount>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
