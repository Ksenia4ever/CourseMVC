using CourseDomain.Model;
using CourseInfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseInfrastructure.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class ScoresController : Controller
    {
        private readonly DbCourseContext _context;

        public ScoresController(DbCourseContext context)
        {
            _context = context;
        }

        // GET: Scores
        public async Task<IActionResult> Index()
        {
            var dbCourseContext = _context.Scores.Include(s => s.Excersice);
            return View(await dbCourseContext.ToListAsync());
        }

        // GET: Scores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var score = await _context.Scores
                .Include(s => s.Excersice)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (score == null)
            {
                return NotFound();
            }

            return View(score);
        }

        // GET: Scores/Create
        public IActionResult Create()
        {
            ViewData["ExcersiceId"] = new SelectList(_context.Excercises, "Id", "Answer");
            return View();
        }

        // POST: Scores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExcersiceId,Value,Id")] Score score)
        {
            if (ModelState.IsValid)
            {
                _context.Add(score);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ExcersiceId"] = new SelectList(_context.Excercises, "Id", "Answer", score.ExcersiceId);
            return View(score);
        }

        // GET: Scores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var score = await _context.Scores.FindAsync(id);
            if (score == null)
            {
                return NotFound();
            }

            ViewData["ExcersiceId"] = new SelectList(_context.Excercises, "Id", "Answer", score.ExcersiceId);
            return View(score);
        }

        // POST: Scores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExcersiceId,Value,Id")] Score score)
        {
            if (id != score.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(score);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScoreExists(score.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ExcersiceId"] = new SelectList(_context.Excercises, "Id", "Answer", score.ExcersiceId);
            return View(score);
        }

        // GET: Scores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var score = await _context.Scores
                .Include(s => s.Excersice)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (score == null)
            {
                return NotFound();
            }

            return View(score);
        }

        // POST: Scores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var score = await _context.Scores.FindAsync(id);

            if (score != null)
            {
                _context.Scores.Remove(score);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScoreExists(int id)
        {
            return _context.Scores.Any(e => e.Id == id);
        }
    }
}