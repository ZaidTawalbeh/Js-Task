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
    public class UserrsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public UserrsController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        // GET: Userrs
        public async Task<IActionResult> Index()
        {
              return _context.Userrs != null ? 
                          View(await _context.Userrs.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Userrs'  is null.");
        }

        // GET: Userrs/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Userrs == null)
            {
                return NotFound();
            }

            var userr = await _context.Userrs
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userr == null)
            {
                return NotFound();
            }

            return View(userr);
        }

        // GET: Userrs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Userrs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Fname,Lname,Phone,Emale,ImageFile")] Userr userr)
        {
            if (ModelState.IsValid)
            {
                if (userr.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" +
                   userr.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/",
                   fileName);
                    using (var fileStream = new FileStream(path,
                   FileMode.Create))
                    {
                        await userr.ImageFile.CopyToAsync(fileStream);
                    }
                    userr.ImagePath = fileName;
                }
                _context.Add(userr);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userr);
        }

        // GET: Userrs/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Userrs == null)
            {
                return NotFound();
            }

            var userr = await _context.Userrs.FindAsync(id);
            if (userr == null)
            {
                return NotFound();
            }
            return View(userr);
        }

        // POST: Userrs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("UserId,Fname,Lname,Phone,Emale,ImageFile")] Userr userr)
        {
            if (id != userr.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (userr.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                       userr.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/",
                       fileName);
                        using (var fileStream = new FileStream(path,
                       FileMode.Create))
                        {
                            await userr.ImageFile.CopyToAsync(fileStream);
                        }
                        userr.ImagePath = fileName;
                    }
                    _context.Update(userr);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserrExists(userr.UserId))
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
            return View(userr);
        }

        // GET: Userrs/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Userrs == null)
            {
                return NotFound();
            }

            var userr = await _context.Userrs
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (userr == null)
            {
                return NotFound();
            }

            return View(userr);
        }

        // POST: Userrs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Userrs == null)
            {
                return Problem("Entity set 'ModelContext.Userrs'  is null.");
            }
            var userr = await _context.Userrs.FindAsync(id);
            if (userr != null)
            {
                _context.Userrs.Remove(userr);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserrExists(decimal id)
        {
          return (_context.Userrs?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
