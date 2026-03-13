using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseInfrastructure;

namespace CourseMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly DbCourseContext _context;

        public ChartsController(DbCourseContext context)
        {
            _context = context;
        }

        [HttpGet("certificatesByYearForCourse/{courseId}")]
        public async Task<JsonResult> GetCertificatesByYearForCourseAsync(int courseId, CancellationToken cancellationToken)
        {
            var result = await _context.Certificates
                .Where(c => c.CourseId == courseId)
                .GroupBy(c => c.IssuedDate.Year)
                .Select(group => new
                {
                    year = group.Key.ToString(),
                    count = group.Count()
                })
                .OrderBy(x => x.year)
                .ToListAsync(cancellationToken);

            return new JsonResult(result);
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