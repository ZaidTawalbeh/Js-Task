using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelMate.Models;

namespace TravelMate.Controllers
{
    public class TestmonialsController : Controller
    {
        private readonly ModelContext _context;

        public TestmonialsController(ModelContext context)
        {
            _context = context;
        }
        // GET: Testmonials
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Testmonials.Include(t => t.User);
            return View(await modelContext.ToListAsync());
        }

        public async Task<IActionResult> Testmonial()
        {
            var latestRecordh = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecordh != null)
            {
                ViewBag.logo = latestRecordh.Imagelogo;
                ViewBag.Imagemain = latestRecordh.Imagemain;
                ViewBag.pWelcom = latestRecordh.PWelcome;
                ViewBag.pFooter = latestRecordh.PFooter;
                ViewBag.copy = latestRecordh.PCopyrigth;
            }
            var latestRecord = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();

            if (latestRecord != null)
            {
                ViewBag.location = latestRecord.Location;
                ViewBag.phone = latestRecord.Phone;
                ViewBag.email = latestRecord.Email;
            }
            var Testmonials = _context.Testmonials.Where(x=>x.Status == "Approved").Include(_ => _.User).ToList();
            ViewBag.Testmonial = Testmonials;
            return View(Testmonials);
        }
        // GET: Testmonials/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Testmonials == null)
            {
                return NotFound();
            }

            var testmonial = await _context.Testmonials
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testmonial == null)
            {
                return NotFound();
            }

            return View(testmonial);
        }

        // GET: Testmonials/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Userrs, "UserId", "UserId");
            return View();
        }

        // POST: Testmonials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Msg,Status,UserId")] Testmonial testmonial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(testmonial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Userrs, "UserId", "UserId", testmonial.UserId);
            return View(testmonial);
        }

        // GET: Testmonials/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Testmonials == null)
            {
                return NotFound();
            }

            var testmonial = await _context.Testmonials.FindAsync(id);
            if (testmonial == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Userrs, "UserId", "UserId", testmonial.UserId);
            return View(testmonial);
        }

        // POST: Testmonials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Msg,Status,UserId")] Testmonial testmonial)
        {
            if (id != testmonial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testmonial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestmonialExists(testmonial.Id))
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
            ViewData["UserId"] = new SelectList(_context.Userrs, "UserId", "UserId", testmonial.UserId);
            return View(testmonial);
        }

        // GET: Testmonials/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Testmonials == null)
            {
                return NotFound();
            }

            var testmonial = await _context.Testmonials
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (testmonial == null)
            {
                return NotFound();
            }

            return View(testmonial);
        }

        // POST: Testmonials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Testmonials == null)
            {
                return Problem("Entity set 'ModelContext.Testmonials'  is null.");
            }
            var testmonial = await _context.Testmonials.FindAsync(id);
            if (testmonial != null)
            {
                _context.Testmonials.Remove(testmonial);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TestmonialExists(decimal id)
        {
          return (_context.Testmonials?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
