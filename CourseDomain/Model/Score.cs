using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseDomain.Model;

public partial class Score : Entity
{
    public int ExcersiceId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Оцінка")]
    public int Value { get; set; }

    public virtual ICollection<AccountScourse> AccountScourses { get; set; } = new List<AccountScourse>();

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Код завдання")]
    public virtual Excercise Excersice { get; set; } = null!;
}
