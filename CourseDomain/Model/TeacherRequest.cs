using System.ComponentModel.DataAnnotations;

namespace CourseDomain.Model;

public partial class TeacherRequest : Entity
{
    [Required]
    public string IdentityUserId { get; set; } = null!;

    [Required]
    [Display(Name = "Ім'я")]
    public string Name { get; set; } = null!;

    [Required]
    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Required]
    [Display(Name = "Статус")]
    public string Status { get; set; } = "Pending";

    [Display(Name = "Дата заявки")]
    public DateTime RequestedAt { get; set; }

    [Display(Name = "Повідомлення адміністратора")]
    public string? AdminMessage { get; set; }

    public virtual ICollection<TeacherRequestCourse> TeacherRequestCourses { get; set; } = new List<TeacherRequestCourse>();
}