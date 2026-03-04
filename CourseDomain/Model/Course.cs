using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseDomain.Model;

public partial class Course : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Назва")]
    public string Title { get; set; } = null!;

    [Display(Name = "Інформація про курс")]
    public string? Description { get; set; } 

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int AuthorId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Предмет")]
    public string Subject { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Дата створення")]
    public DateOnly Created { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Дата редагування")]
    public DateOnly Modified { get; set; }

   
    [Display(Name = "Код автора")]
    public virtual Account? Author { get; set; }

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual ICollection<CourseAccount> CourseAccounts { get; set; } = new List<CourseAccount>();

    public virtual ICollection<Excercise> Excercises { get; set; } = new List<Excercise>();
}
