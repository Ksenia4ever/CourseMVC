using CourseDomain.Model;
using CourseInfrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CourseInfrastructure.Services.IDataPortServiceFactory;

namespace CourseInfrastructure.Controllers
{
    [SessionAuthorize]
    public class CoursesController : Controller
    {
        private readonly DbCourseContext _context;
        private readonly IDataPortServiceFactory<CourseAccount> _dataPortServiceFactory;

        public CoursesController(
            DbCourseContext context,
            IDataPortServiceFactory<CourseAccount> dataPortServiceFactory)
        {
            _context = context;
            _dataPortServiceFactory = dataPortServiceFactory;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var currentAccountId = HttpContext.Session.GetInt32("CurrentAccountId");
            ViewBag.CurrentAccountId = currentAccountId;

            var dbCourseContext = _context.Courses
                .Include(c => c.Author)
                .Include(c => c.CourseAccounts)
                .OrderBy(c => c.Title);

            return View(await dbCourseContext.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Author)
                .Include(c => c.Certificates)
                .Include(c => c.CourseAccounts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Accounts, "Id", "Name");
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,AuthorId,Subject")] Course course)
        {
            course.Created = DateOnly.FromDateTime(DateTime.Today);
            course.Modified = DateOnly.FromDateTime(DateTime.Today);

            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.Accounts, "Id", "Name", course.AuthorId);
            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            ViewData["AuthorId"] = new SelectList(_context.Accounts, "Id", "Name", course.AuthorId);
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,AuthorId,Subject,Created")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            course.Modified = DateOnly.FromDateTime(DateTime.Today);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.Accounts, "Id", "Name", course.AuthorId);
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Courses/ToggleSubscription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleSubscription(int courseId)
        {
            var accountId = HttpContext.Session.GetInt32("CurrentAccountId");

            if (accountId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var existingSubscription = await _context.CourseAccounts
                .FirstOrDefaultAsync(ca => ca.CourseId == courseId && ca.AccountId == accountId.Value);

            if (existingSubscription == null)
            {
                var subscription = new CourseAccount
                {
                    CourseId = courseId,
                    AccountId = accountId.Value,
                    SubscribedAt = DateTime.Now
                };

                _context.CourseAccounts.Add(subscription);
            }
            else
            {
                _context.CourseAccounts.Remove(existingSubscription);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: Courses/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel, CancellationToken cancellationToken)
        {
            if (fileExcel == null || fileExcel.Length == 0)
            {
                ModelState.AddModelError("", "Оберіть Excel-файл для імпорту.");
                return View();
            }

            var importService = _dataPortServiceFactory.GetImportService(fileExcel.ContentType);

            using var stream = fileExcel.OpenReadStream();
            await importService.ImportFromStreamAsync(stream, cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        // GET: Courses/Export
        [HttpGet]
        public async Task<IActionResult> Export(
            [FromQuery] string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            CancellationToken cancellationToken = default)
        {
            var exportService = _dataPortServiceFactory.GetExportService(contentType);

            var memoryStream = new MemoryStream();

            await exportService.WriteToAsync(memoryStream, cancellationToken);

            await memoryStream.FlushAsync(cancellationToken);
            memoryStream.Position = 0;

            return new FileStreamResult(memoryStream, contentType)
            {
                FileDownloadName = $"course_subscriptions_{DateTime.UtcNow:yyyy-MM-dd}.xlsx"
            };
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}