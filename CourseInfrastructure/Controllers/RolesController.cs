using CourseDomain.Model;
using CourseInfrastructure.Models;
using CourseInfrastructure.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseInfrastructure.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public RolesController(
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // список усіх ролей
        public IActionResult Index()
        {
            return View(_roleManager.Roles.ToList());
        }

        // список усіх користувачів
        public IActionResult UserList()
        {
            return View(_userManager.Users.ToList());
        }

        // GET: Roles/Edit/{userId}
        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            ChangeRoleViewModel model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email ?? "",
                UserName = user.Name,
                UserRoles = userRoles,
                AllRoles = allRoles
            };

            return View(model);
        }

        // POST: Roles/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            roles ??= new List<string>();

            var addedRoles = roles.Except(userRoles);
            var removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            return RedirectToAction(nameof(UserList));
        }
    }
}