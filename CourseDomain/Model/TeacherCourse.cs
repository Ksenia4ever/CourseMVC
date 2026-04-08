using System.ComponentModel.DataAnnotations;

namespace CourseDomain.Model;

public partial class TeacherCourse : Entity
{
    [Required]
    public int AccountId { get; set; }

    [Required]
    public int CourseId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}