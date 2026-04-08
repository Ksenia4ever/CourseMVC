using CourseDomain.Model;
using CourseInfrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseInfrastructure.Controllers
{
    [Authorize(Roles = "Student,Teacher,Admin")]
    public class CertificatesController : Controller
    {
        private readonly DbCourseContext _context;

        public CertificatesController(DbCourseContext context)
        {
            _context = context;
        }

        // GET: Certificates
        public async Task<IActionResult> Index(int? id, string? name)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Accounts");
            }

            ViewBag.AccountId = id;
            ViewBag.AccountName = name;

            var certificateByAccount = _context.Certificates
                .Where(x => x.AccountId == id)
                .Include(x => x.Account)
                .Include(x => x.Course);

            return View(await certificateByAccount.ToListAsync());
        }

        // GET: Certificates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificates
                .Include(c => c.Account)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        // GET: Certificates/Create
        public IActionResult Create(int id, string? name)
        {
            ViewBag.AccountId = id;
            ViewBag.AccountName = name;
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title");

            return View();
        }

        // POST: Certificates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,CourseId,AccountId")] Certificate certificate)
        {
            bool exists = await _context.Certificates.AnyAsync(c =>
                c.AccountId == certificate.AccountId &&
                c.CourseId == certificate.CourseId);

            if (exists)
            {
                ModelState.AddModelError("", "Цей акаунт вже отримав сертифікат за цей курс.");
            }

            if (ModelState.IsValid)
            {
                certificate.IssuedDate = DateTime.Now;
                _context.Add(certificate);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { id = certificate.AccountId });
            }

            ViewBag.AccountId = certificate.AccountId;
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title", certificate.CourseId);

            return View(certificate);
        }

        // GET: Certificates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }

            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Name", certificate.AccountId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", certificate.CourseId);

            return View(certificate);
        }

        // POST: Certificates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Description,CourseId,AccountId,Id,IssuedDate")] Certificate certificate)
        {
            if (id != certificate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(certificate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificateExists(certificate.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index), new { id = certificate.AccountId });
            }

            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Name", certificate.AccountId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Title", certificate.CourseId);

            return View(certificate);
        }

        // GET: Certificates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificates
                .Include(c => c.Account)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (certificate == null)
            {
                return NotFound();
            }

            return View(certificate);
        }

        // POST: Certificates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);

            if (certificate == null)
            {
                return RedirectToAction("Index", "Accounts");
            }

            int accId = certificate.AccountId;

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = accId });
        }

        private bool CertificateExists(int id)
        {
            return _context.Certificates.Any(e => e.Id == id);
        }

        public async Task<IActionResult> MyCertificates()
        {
            var identityUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(identityUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.IdentityUserId == identityUserId);

            if (account == null)
            {
                return NotFound();
            }

            var certificates = await _context.Certificates
                .Where(c => c.AccountId == account.Id)
                .Include(c => c.Account)
                .Include(c => c.Course)
                .ToListAsync();

            ViewBag.AccountId = account.Id;
            ViewBag.AccountName = account.Name;

            return View("Index", certificates);
        }
    }
}