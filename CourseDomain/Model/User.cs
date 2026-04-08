using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CourseDomain.Model
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Поле імені не повинно бути порожнім")]
        [Display(Name = "Ім'я")]
        public string Name { get; set; } = null!;
    }
}