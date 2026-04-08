using System.ComponentModel.DataAnnotations;

namespace CourseDomain.Model;

public partial class TeacherRequestCourse : Entity
{
    [Required]
    public int TeacherRequestId { get; set; }

    [Required]
    public int CourseId { get; set; }

    public virtual TeacherRequest TeacherRequest { get; set; } = null!;

    public virtual Course Course { get; set; } = null!;
}