using CourseInfrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseInfrastructure;
using System.Diagnostics;

namespace CourseInfrastructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbCourseContext _context;

        public HomeController(DbCourseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                TempData["LoginError"] = "Введіть ім'я користувача та електронну пошту.";
                return RedirectToAction(nameof(Index));
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Name == name && a.Email == email);

            if (account == null)
            {
                TempData["LoginError"] = "Неправильне ім'я користувача або електронна пошта.";
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.SetInt32("CurrentAccountId", account.Id);
            HttpContext.Session.SetString("CurrentAccountEmail", account.Email);
            HttpContext.Session.SetString("CurrentAccountName", account.Name);

            return RedirectToAction("Index", "Courses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StayLoggedIn()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetLogin()
        {
            HttpContext.Session.Remove("CurrentAccountId");
            HttpContext.Session.Remove("CurrentAccountEmail");
            HttpContext.Session.Remove("CurrentAccountName");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CurrentAccountId");
            HttpContext.Session.Remove("CurrentAccountEmail");
            HttpContext.Session.Remove("CurrentAccountName");

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [SessionAuthorize]
        public IActionResult AdminStats()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}