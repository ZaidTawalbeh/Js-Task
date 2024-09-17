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
    public class EventsController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public EventsController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Events.Include(x => x.Hotel);
            return View(await modelContext.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(x => x.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["HotelId"] = new SelectList(_context.Hotels, "HotelId", "HotelId");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Eventname,Eventdescription,ImageFile,HotelId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                if (@event.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" +
                   @event.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/",
                   fileName);
                    using (var fileStream = new FileStream(path,
                   FileMode.Create))
                    {
                        await @event.ImageFile.CopyToAsync(fileStream);
                    }
                    @event.ImagePath = fileName;
                }
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "HotelId", "HotelId", @event.HotelId);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["HotelId"] = new SelectList(_context.Hotels, "HotelId", "HotelId", @event.HotelId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Eventname,Eventdescription,ImageFile,HotelId")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (@event.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                       @event.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/",
                       fileName);
                        using (var fileStream = new FileStream(path,
                       FileMode.Create))
                        {
                            await @event.ImageFile.CopyToAsync(fileStream);
                        }
                        @event.ImagePath = fileName;
                    }
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            ViewData["HotelId"] = new SelectList(_context.Hotels, "HotelId", "HotelId", @event.HotelId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Events == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(x => x.Hotel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Events == null)
            {
                return Problem("Entity set 'ModelContext.Events'  is null.");
            }
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(decimal id)
        {
          return (_context.Events?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
