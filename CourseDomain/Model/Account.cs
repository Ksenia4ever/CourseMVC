using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseDomain.Model;

public partial class Account : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Ім'я")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [EmailAddress(ErrorMessage = "Введіть коректну email-адресу")]
    [Display(Name = "Електронна пошта")]
    public string Email { get; set; } = null!;

    [Display(Name = "Identity User Id")]
    public string? IdentityUserId { get; set; }

    [Display(Name = "Системне повідомлення")]
    public string? SystemMessage { get; set; }

    public bool HasUnreadSystemMessage { get; set; }
    public virtual ICollection<AccountScourse> AccountScourses { get; set; } = new List<AccountScourse>();

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual ICollection<CourseAccount> CourseAccounts { get; set; } = new List<CourseAccount>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<TeacherCourse> TeacherCourses { get; set; } = new List<TeacherCourse>();
}