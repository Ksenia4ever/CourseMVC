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

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AdminStats()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Введіть електронну пошту.";
                return View();
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Email == email);

            if (account == null)
            {
                ViewBag.Error = "Акаунт з такою поштою не знайдено.";
                return View();
            }

            HttpContext.Session.SetInt32("CurrentAccountId", account.Id);
            HttpContext.Session.SetString("CurrentAccountEmail", account.Email);
            HttpContext.Session.SetString("CurrentAccountName", account.Name);

            return RedirectToAction("Index", "Courses");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CurrentAccountId");
            HttpContext.Session.Remove("CurrentAccountEmail");
            HttpContext.Session.Remove("CurrentAccountName");

            return RedirectToAction(nameof(Login));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}