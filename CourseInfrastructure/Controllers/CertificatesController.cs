using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseDomain.Model;
using CourseInfrastructure;

namespace CourseInfrastructure.Controllers
{
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
            if (id == null) return RedirectToAction("Accounts", "Index");
            //перегляд вже отриманіх сертифікатів
            ViewBag.AccountId = id;
            ViewBag.AccountName = name;
            var certificateByAccount = _context.Certificates.Where(x => x.AccountId == id).Include(x => x.Account);
            return View(await certificateByAccount.ToListAsync());
            //var dbCourseContext = _context.Certificates.Include(c => c.Account).Include(c => c.Course);
            //return View(await dbCourseContext.ToListAsync());
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
        public IActionResult Create()
        {
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Name");
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description");
            return View();
        }
        //public IActionResult Create(int accountId)
        //{
        //    ViewBag.AccountId = accountId;

        //    // если хочешь показать имя аккаунта в заголовке
        //    var account = _context.Accounts.Find(accountId);
        //    ViewBag.AccountName = account?.Name; // подставь своё поле

        //    ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name"); // подставь своё поле названия курса

        //    return View();
        //}

        // POST: Certificates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,CourseId,AccountId,Id")] Certificate certificate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(certificate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Name", certificate.AccountId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", certificate.CourseId);
            return View(certificate);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(Certificate certificate)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title", certificate.CourseId);
        //        ViewBag.AccountId = certificate.AccountId;
        //        return View(certificate);
        //    }

        //    _context.Certificates.Add(certificate);

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        // сработает, если нарушится уникальность (AccountId, CourseId)
        //        ModelState.AddModelError("", "Цей акаунт вже має сертифікат за обраний курс.");
        //        ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Title", certificate.CourseId);
        //        return View(certificate);
        //    }

        //    return RedirectToAction("Details", "Accounts", new { id = certificate.AccountId });
        //}

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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", certificate.CourseId);
            return View(certificate);
        }

        // POST: Certificates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Title,Description,CourseId,AccountId,Id")] Certificate certificate)
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
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AccountId"] = new SelectList(_context.Accounts, "Id", "Name", certificate.AccountId);
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Description", certificate.CourseId);
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
            if (certificate != null)
            {
                _context.Certificates.Remove(certificate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CertificateExists(int id)
        {
            return _context.Certificates.Any(e => e.Id == id);
        }
    }
}
