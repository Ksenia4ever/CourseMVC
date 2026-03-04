using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseDomain.Model;

public partial class Excercise : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Назва")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Завдання")]
    public string TaskDescription { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Питання")]
    public string Questions { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Варіанті відповіді")]
    public string AnswerVarients { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Відповідь")]
    public string Answer { get; set; } = null!;

    public int CourseId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Дата створення")]
    public DateOnly Created { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Дата редагування")]
    public DateOnly Modified { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Код курсу")]
    public virtual Course Course { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Код оцінки")]
    public virtual Score? Score { get; set; }
}
