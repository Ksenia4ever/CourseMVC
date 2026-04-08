using CourseDomain.Model;
using CourseInfrastructure.ViewModel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseInfrastructure.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DbCourseContext _context;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            DbCourseContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Courses = _context.Courses
                .OrderBy(c => c.Title)
                .ToList();

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewBag.Courses = _context.Courses
                .OrderBy(c => c.Title)
                .ToList();

            if (ModelState.IsValid)
            {
                bool emailExistsInIdentity = await _userManager.FindByEmailAsync(model.Email) != null;
                bool emailExistsInAccounts = await _context.Accounts.AnyAsync(a => a.Email == model.Email);

                if (emailExistsInIdentity || emailExistsInAccounts)
                {
                    ModelState.AddModelError(string.Empty, "Користувач з таким email уже існує.");
                    return View(model);
                }

                if (model.SelectedRole == "Teacher" && (model.SelectedCourseIds == null || model.SelectedCourseIds.Count == 0))
                {
                    ModelState.AddModelError(string.Empty, "Для ролі вчителя потрібно обрати хоча б один курс.");
                    return View(model);
                }

                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");

                    var account = new Account
                    {
                        Name = model.Name,
                        Email = model.Email,
                        IdentityUserId = user.Id
                    };

                    _context.Accounts.Add(account);
                    await _context.SaveChangesAsync();

                    if (model.SelectedRole == "Teacher")
                    {
                        var teacherRequest = new TeacherRequest
                        {
                            IdentityUserId = user.Id,
                            Name = model.Name,
                            Email = model.Email,
                            RequestedAt = DateTime.Now,
                            Status = "Pending"
                        };

                        _context.Add(teacherRequest);
                        await _context.SaveChangesAsync();

                        foreach (var courseId in model.SelectedCourseIds)
                        {
                            _context.Add(new TeacherRequestCourse
                            {
                                TeacherRequestId = teacherRequest.Id,
                                CourseId = courseId
                            });
                        }

                        await _context.SaveChangesAsync();
                    }

                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Courses");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user != null)
                    {
                        var account = await _context.Accounts
                            .FirstOrDefaultAsync(a => a.IdentityUserId == user.Id);

                        if (account != null && account.HasUnreadSystemMessage && !string.IsNullOrEmpty(account.SystemMessage))
                        {
                            TempData["SystemMessage"] = account.SystemMessage;
                            account.HasUnreadSystemMessage = false;
                            await _context.SaveChangesAsync();
                        }
                    }

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Courses");
                }

                ModelState.AddModelError(string.Empty, "Неправильний email або пароль");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

       
    }
}