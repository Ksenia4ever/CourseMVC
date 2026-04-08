using CourseDomain.Model;
using CourseInfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseInfrastructure.Controllers
{
    [Authorize(Roles = "Student,Teacher,Admin")]
    public class ExcercisesController : Controller
    {
        private readonly DbCourseContext _context;

        public ExcercisesController(DbCourseContext context)
        {
            _context = context;
        }

        // GET: Excercises
        public async Task<IActionResult> Index(int? courseId)
        {
            var query = _context.Excercises
                .Include(e => e.Course)
                .AsQueryable();

            if (courseId.HasValue)
            {
                query = query.Where(e => e.CourseId == courseId.Value);
                ViewBag.CourseId = courseId.Value;

                var course = await _context.Courses.FindAsync(courseId.Value);
                ViewBag.CourseTitle = course?.Title;
            }

            return View(await query.ToListAsync());
        }

        // GET: Excercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var excercise = await _context.Excercises
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (excercise == null)
            {
                return NotFound();
            }

            return View(excercise);
        }

        // GET: Excercises/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description");
            return View();
        }

        // POST: Excercises/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,TaskDescription,Questions,AnswerVarients,Answer,CourseId,Created,Modified,Id")] Excercise excercise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(excercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", excercise.CourseId);
            return View(excercise);
        }

        // GET: Excercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var excercise = await _context.Excercises.FindAsync(id);
            if (excercise == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", excercise.CourseId);
            return View(excercise);
        }

        // POST: Excercises/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,TaskDescription,Questions,AnswerVarients,Answer,CourseId,Created,Modified,Id")] Excercise excercise)
        {
            if (id != excercise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(excercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExcerciseExists(excercise.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", excercise.CourseId);
            return View(excercise);
        }

        // GET: Excercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var excercise = await _context.Excercises
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (excercise == null)
            {
                return NotFound();
            }

            return View(excercise);
        }

        // POST: Excercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var excercise = await _context.Excercises.FindAsync(id);

            if (excercise != null)
            {
                _context.Excercises.Remove(excercise);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExcerciseExists(int id)
        {
            return _context.Excercises.Any(e => e.Id == id);
        }
    }
}