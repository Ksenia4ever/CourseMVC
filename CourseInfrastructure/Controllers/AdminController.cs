using CourseDomain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseInfrastructure.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DbCourseContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(DbCourseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> TeacherRequests()
        {
            var requests = await _context.TeacherRequests
                .Include(r => r.TeacherRequestCourses)
                    .ThenInclude(rc => rc.Course)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            return View(requests);
        }

        public async Task<IActionResult> TeacherRequestInfo(int id)
        {
            var request = await _context.TeacherRequests
                .Include(r => r.TeacherRequestCourses)
                    .ThenInclude(rc => rc.Course)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveTeacherRequest(int id)
        {
            var request = await _context.TeacherRequests
                .Include(r => r.TeacherRequestCourses)
                    .ThenInclude(rc => rc.Course)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(request.IdentityUserId);
            if (user == null)
            {
                return NotFound();
            }

            if (!await _userManager.IsInRoleAsync(user, "Teacher"))
            {
                await _userManager.AddToRoleAsync(user, "Teacher");
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.IdentityUserId == request.IdentityUserId);

            if (account == null)
            {
                return NotFound();
            }

            foreach (var item in request.TeacherRequestCourses)
            {
                bool exists = await _context.TeacherCourses
                    .AnyAsync(tc => tc.AccountId == account.Id && tc.CourseId == item.CourseId);

                if (!exists)
                {
                    _context.TeacherCourses.Add(new TeacherCourse
                    {
                        AccountId = account.Id,
                        CourseId = item.CourseId
                    });
                }
            }

            var courseTitles = request.TeacherRequestCourses
                .Select(x => x.Course.Title)
                .ToList();

            account.SystemMessage =
                $"Вашу заявку на викладання курсів: {string.Join(", ", courseTitles)} прийнято.";
            account.HasUnreadSystemMessage = true;

            _context.TeacherRequestCourses.RemoveRange(request.TeacherRequestCourses);
            _context.TeacherRequests.Remove(request);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(TeacherRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectTeacherRequest(int id)
        {
            var request = await _context.TeacherRequests
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            request.Status = "Rejected";
            request.AdminMessage = "Вам відмовлено у наданні доступу викладача.";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TeacherRequests));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTeacherRequest(int id)
        {
            var request = await _context.TeacherRequests
                .Include(r => r.TeacherRequestCourses)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            _context.TeacherRequestCourses.RemoveRange(request.TeacherRequestCourses);
            _context.TeacherRequests.Remove(request);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(TeacherRequests));
        }
    }
}