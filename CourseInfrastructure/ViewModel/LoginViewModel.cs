using System.ComponentModel.DataAnnotations;

namespace CourseInfrastructure.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Поле email є обов'язковим")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Поле пароля є обов'язковим")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}