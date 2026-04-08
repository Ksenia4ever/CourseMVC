using Microsoft.AspNetCore.Identity;

namespace CourseInfrastructure.ViewModel
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; } = null!;

        public string UserEmail { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public List<IdentityRole> AllRoles { get; set; }

        public IList<string> UserRoles { get; set; }

        public ChangeRoleViewModel()
        {
            AllRoles = new List<IdentityRole>();
            UserRoles = new List<string>();
        }
    }
}