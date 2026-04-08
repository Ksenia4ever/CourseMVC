using System.ComponentModel.DataAnnotations;

namespace CourseInfrastructure.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Поле імені є обов'язковим")]
        [Display(Name = "Ім'я")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Поле email є обов'язковим")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Поле пароля є обов'язковим")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Підтвердження пароля є обов'язковим")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        [Display(Name = "Підтвердження пароля")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; } = null!;

        [Required(ErrorMessage = "Оберіть тип акаунта")]
        [Display(Name = "Тип акаунта")]
        public string SelectedRole { get; set; } = "Student";

        public List<int> SelectedCourseIds { get; set; } = new();
    }
}