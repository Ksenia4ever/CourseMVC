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

        [HttpGet("courseDetailsStats/{courseId}")]
        public async Task<JsonResult> GetCourseDetailsStatsAsync(int courseId, CancellationToken cancellationToken)
        {
            var excercisesCount = await _context.Excercises
                .CountAsync(e => e.CourseId == courseId, cancellationToken);

            var certificatesCount = await _context.Certificates
                .CountAsync(c => c.CourseId == courseId, cancellationToken);

            var result = new[]
            {
                new { label = "Вправи", count = excercisesCount },
                new { label = "Сертифікати", count = certificatesCount }
            };

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