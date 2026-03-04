using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CourseDomain.Model;

public partial class Certificate : Entity
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name="Назва")]
    public string Title { get; set; } = null!;

    [Display(Name="Інформація про сертифікат")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Код курсу")]
    public int CourseId { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Код аккаунту")]
    public int AccountId { get; set; }

   
   // [Display(Name = "Код аккаунту")]
    public virtual Account Account { get; set; } = null!;

    
   // [Display(Name = "Код курсу")]
    public virtual Course Course { get; set; } = null!;
}
