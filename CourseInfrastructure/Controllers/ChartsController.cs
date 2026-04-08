using CourseInfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CourseInfrastructure.Controllers
{
    [Authorize(Roles = "Student,Teacher,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly DbCourseContext _context;

        public ChartsController(DbCourseContext context)
        {
            _context = context;
        }

        [HttpGet("subscriptionsByMonthForCourse/{courseId}")]
        public async Task<JsonResult> GetSubscriptionsByMonthForCourseAsync(int courseId, CancellationToken cancellationToken)
        {
            var result = await _context.CourseAccounts
                .Where(ca => ca.CourseId == courseId)
                .GroupBy(ca => new { ca.SubscribedAt.Year, ca.SubscribedAt.Month })
                .Select(group => new
                {
                    year = group.Key.Year,
                    month = group.Key.Month,
                    count = group.Count()
                })
                .OrderBy(x => x.year)
                .ThenBy(x => x.month)
                .ToListAsync(cancellationToken);

            var monthNames = new[]
            {
                "",
                "січень",
                "лютий",
                "березень",
                "квітень",
                "травень",
                "червень",
                "липень",
                "серпень",
                "вересень",
                "жовтень",
                "листопад",
                "грудень"
            };

            var formattedResult = result.Select(x => new
            {
                label = $"{monthNames[x.month]} {x.year}",
                count = x.count
            });

            return new JsonResult(formattedResult);
        }

        [HttpGet("systemStats")]
        public async Task<JsonResult> GetSystemStatsAsync(CancellationToken cancellationToken)
        {
            var accountsCount = await _context.Accounts.CountAsync(cancellationToken);
            var coursesCount = await _context.Courses.CountAsync(cancellationToken);
            var excercisesCount = await _context.Excercises.CountAsync(cancellationToken);
            var certificatesCount = await _context.Certificates.CountAsync(cancellationToken);
            var scoresCount = await _context.Scores.CountAsync(cancellationToken);

            var result = new[]
            {
                new { label = "Акаунти", count = accountsCount },
                new { label = "Курси", count = coursesCount },
                new { label = "Вправи", count = excercisesCount },
                new { label = "Сертифікати", count = certificatesCount },
                new { label = "Оцінки", count = scoresCount }
            };

            return new JsonResult(result);
        }
    }
}