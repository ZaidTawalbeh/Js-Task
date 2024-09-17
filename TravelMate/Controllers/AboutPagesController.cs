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
    public class AboutPagesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public AboutPagesController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        // GET: AboutPages
        public async Task<IActionResult> Index()
        {
              return _context.AboutPages != null ? 
                          View(await _context.AboutPages.ToListAsync()) :
                          Problem("Entity set 'ModelContext.AboutPages'  is null.");
        }
        public IActionResult About_Us()
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
            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();

            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.AboutPages.OrderByDescending(c => c.Id).FirstOrDefault();

            if (latestRecord != null)
            {
                ViewBag.Image = latestRecord.Imagemain;
                ViewBag.pAbout = latestRecord.PAbout;
            }

            //********************//
            ViewBag.Imglogo = HttpContext.Session.GetString("Imagelogo");
            ViewBag.Imgmain = HttpContext.Session.GetString("Imagemain");
            ViewBag.welcome = HttpContext.Session.GetString("PWelcome");
            ViewBag.foot = HttpContext.Session.GetString("PFooter");
            ViewBag.cop = HttpContext.Session.GetString("PCopyrigth");
            return View();
        }

        // GET: AboutPages/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.AboutPages == null)
            {
                return NotFound();
            }

            var aboutPage = await _context.AboutPages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aboutPage == null)
            {
                return NotFound();
            }

            return View(aboutPage);
        }

        // GET: AboutPages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AboutPages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImageFile,PAbout")] AboutPage aboutPage)
        {
            if (ModelState.IsValid)
            {
                if (aboutPage.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" +
                   aboutPage.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/",
                   fileName);
                    using (var fileStream = new FileStream(path,
                   FileMode.Create))
                    {
                        await aboutPage.ImageFile.CopyToAsync(fileStream);
                    }
                    aboutPage.Imagemain = fileName;
                }
                _context.Add(aboutPage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aboutPage);
        }

        // GET: AboutPages/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.AboutPages == null)
            {
                return NotFound();
            }

            var aboutPage = await _context.AboutPages.FindAsync(id);
            if (aboutPage == null)
            {
                return NotFound();
            }
            return View(aboutPage);
        }

        // POST: AboutPages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,ImageFile,PAbout")] AboutPage aboutPage)
        {
            if (id != aboutPage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (aboutPage.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                       aboutPage.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/",
                       fileName);
                        using (var fileStream = new FileStream(path,
                       FileMode.Create))
                        {
                            await aboutPage.ImageFile.CopyToAsync(fileStream);
                        }
                        aboutPage.Imagemain = fileName;
                    }
                    _context.Update(aboutPage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutPageExists(aboutPage.Id))
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
            return View(aboutPage);
        }

        // GET: AboutPages/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.AboutPages == null)
            {
                return NotFound();
            }

            var aboutPage = await _context.AboutPages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aboutPage == null)
            {
                return NotFound();
            }

            return View(aboutPage);
        }

        // POST: AboutPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.AboutPages == null)
            {
                return Problem("Entity set 'ModelContext.AboutPages'  is null.");
            }
            var aboutPage = await _context.AboutPages.FindAsync(id);
            if (aboutPage != null)
            {
                _context.AboutPages.Remove(aboutPage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AboutPageExists(decimal id)
        {
          return (_context.AboutPages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
