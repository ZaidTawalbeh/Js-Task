using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TravelMate.Models;
using iTextSharp.text.pdf;
using System.IO;
using System.Net;
using System.Net.Mail;
using iTextSharp.text;


namespace TravelMate.Controllers
{
    public class HomePagesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public HomePagesController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        ///***************************************Book**************************************//
        [HttpGet]
        public IActionResult BookNow(int roomId)
        {
            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);

            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecord != null)
            {
                ViewBag.logo = latestRecord.Imagelogo;
                ViewBag.Imagemain = latestRecord.Imagemain;
                ViewBag.pWelcom = latestRecord.PWelcome;
                ViewBag.pFooter = latestRecord.PFooter;
                ViewBag.copy = latestRecord.PCopyrigth;
            }
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Home", "AskToLog");
            }

            var reservation = new Reservation
            {
                UserId = userId.Value,  
                RoomId = roomId
            };

            return View(reservation); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmBooking(Reservation reservation)
        {
            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);

            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecord != null)
            {
                ViewBag.logo = latestRecord.Imagelogo;
                ViewBag.Imagemain = latestRecord.Imagemain;
                ViewBag.pWelcom = latestRecord.PWelcome;
                ViewBag.pFooter = latestRecord.PFooter;
                ViewBag.copy = latestRecord.PCopyrigth;
            }

            var room = _context.Rooms.Find(reservation.RoomId);
            var totalPrice = room.Pricepernight * (reservation.Checkoutdate.Value.Date - reservation.Checkindate.Value.Date).Days;


            var userId = HttpContext.Session.GetInt32("UserId");
            reservation.UserId = userId.Value;

            var paymentDetails = new PaymentDetails
            {
                Reservation = reservation,
                TotalPrice = (decimal)totalPrice,
                UserId = userId.Value  
            };
            var today = DateTime.Now.Date;

            if (reservation.Checkindate < today || reservation.Checkoutdate <= reservation.Checkindate)
            {
                ModelState.AddModelError(string.Empty, "Please ensure the check-in date is today or later, and the check-out date is after the check-in date.");
                return View("BookNow", reservation); 
            }
            var isRoomAvailable = !_context.Reservations.Any(r =>
                r.RoomId == reservation.RoomId &&
                ((reservation.Checkindate >= r.Checkindate && reservation.Checkindate < r.Checkoutdate) ||
                (reservation.Checkoutdate > r.Checkindate && reservation.Checkoutdate <= r.Checkoutdate) ||
                (reservation.Checkindate < r.Checkindate && reservation.Checkoutdate > r.Checkoutdate))
            );

            if (!isRoomAvailable)
            {
                ModelState.AddModelError(string.Empty, "The room is not available for the selected dates.");
                return View("BookNow", reservation); 
            }

            return View("PaymentDetails", paymentDetails); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Payment(PaymentDetails paymentDetails)
        {
            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);

            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecord != null)
            {
                ViewBag.logo = latestRecord.Imagelogo;
                ViewBag.Imagemain = latestRecord.Imagemain;
                ViewBag.pWelcom = latestRecord.PWelcome;
                ViewBag.pFooter = latestRecord.PFooter;
                ViewBag.copy = latestRecord.PCopyrigth;
            }

            return View("EnterCardDetails", paymentDetails);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(PaymentDetails paymentDetails)
        {
            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);

            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecord != null)
            {
                ViewBag.logo = latestRecord.Imagelogo;
                ViewBag.Imagemain = latestRecord.Imagemain;
                ViewBag.pWelcom = latestRecord.PWelcome;
                ViewBag.pFooter = latestRecord.PFooter;
                ViewBag.copy = latestRecord.PCopyrigth;
            }

            var userId = paymentDetails.UserId;
            var cardNumber = Convert.ToInt64(paymentDetails.CardNumber);

            var bankAccount = await _context.Banks
                .FirstOrDefaultAsync(b => b.Creditnumber == cardNumber && b.Creditexp == paymentDetails.ExpirationDate);

            if (bankAccount == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid card details.");
                return View("EnterCardDetails", paymentDetails);
            }

            if (bankAccount.Balance >= paymentDetails.TotalPrice)
            {
                bankAccount.Balance -= paymentDetails.TotalPrice;
                await _context.SaveChangesAsync();

                var userExists = await _context.Userrs.AnyAsync(u => u.UserId == userId);
                if (!userExists)
                {
                    ModelState.AddModelError(string.Empty, "User does not exist.");
                    return View("EnterCardDetails", paymentDetails);
                }

                var reservation = new Reservation
                {
                    RoomId = paymentDetails.Reservation.RoomId,
                    Checkindate = paymentDetails.Reservation.Checkindate,
                    Checkoutdate = paymentDetails.Reservation.Checkoutdate,
                    UserId = userId 
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();


                byte[] pdfInvoice = GenerateInvoicePDF(paymentDetails);

                var user = await _context.Userrs.FindAsync(userId);
                if (user != null)
                {
                    SendInvoiceEmail(user.Emale, pdfInvoice);
                }
                return RedirectToAction(nameof(UserHome));  

            }
            else
            {
                ModelState.AddModelError(string.Empty, "Insufficient balance.");
                return View("EnterCardDetails", paymentDetails);
            }
        }

        public byte[] GenerateInvoicePDF(PaymentDetails paymentDetails)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 50, 50, 25, 25);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                var titleFont = FontFactory.GetFont("Arial", 18, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY); 
                var headerFont = FontFactory.GetFont("Arial", 12, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
                var bodyFont = FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.NORMAL, BaseColor.BLACK); 

                Paragraph title = new Paragraph("Invoice", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);
                document.Add(new Paragraph(" ")); 

                PdfPTable table = new PdfPTable(2); 
                table.WidthPercentage = 100; 

                AddColoredTableCell(table, "Reservation for Room:", headerFont, bodyFont, paymentDetails.Reservation.RoomId.ToString());
                AddColoredTableCell(table, "Check-in Date:", headerFont, bodyFont, paymentDetails.Reservation.Checkindate.HasValue ? paymentDetails.Reservation.Checkindate.Value.ToShortDateString() : "N/A");
                AddColoredTableCell(table, "Check-out Date:", headerFont, bodyFont, paymentDetails.Reservation.Checkoutdate.HasValue ? paymentDetails.Reservation.Checkoutdate.Value.ToShortDateString() : "N/A");
                AddColoredTableCell(table, "Total Price:", headerFont, bodyFont, $"${paymentDetails.TotalPrice}");

                document.Add(table);

                document.Close();
                writer.Close();

                return ms.ToArray();
            }
        }

        private void AddColoredTableCell(PdfPTable table, string headerText, Font headerFont, Font bodyFont, string bodyText)
        {
            PdfPCell headerCell = new PdfPCell(new Phrase(headerText, headerFont));
            headerCell.BackgroundColor = BaseColor.DARK_GRAY;  
            headerCell.BorderColor = BaseColor.BLACK; 
            headerCell.Padding = 5;  
            table.AddCell(headerCell);

            PdfPCell bodyCell = new PdfPCell(new Phrase(bodyText, bodyFont));
            bodyCell.BorderColor = BaseColor.BLACK;
            bodyCell.Padding = 5; 
            table.AddCell(bodyCell);
        }

      
        public void SendInvoiceEmail(string userEmail, byte[] pdfContent)
        {
            var latestRecord = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();

            var myemail = latestRecord.Email;

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(myemail);
                mail.To.Add(userEmail);
                mail.Subject = "Your Reservation Invoice";
                mail.Body = "Please find your invoice attached.";

                mail.Attachments.Add(new Attachment(new MemoryStream(pdfContent), "Invoice.pdf"));

                using (SmtpClient smtp = new SmtpClient("smtp.office365.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(myemail, "Zaid1996");
                    smtp.EnableSsl = true;
                    try
                    {
                        smtp.Send(mail); 
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Failed to send email.", ex);
                    }
                }
            }
        }


        //******************************search*****************************

        [HttpGet]
        public IActionResult Search(string searchTerm)
        {
            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);

            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecord != null)
            {
                ViewBag.logo = latestRecord.Imagelogo;
                ViewBag.Imagemain = latestRecord.Imagemain;
                ViewBag.pWelcom = latestRecord.PWelcome;
                ViewBag.pFooter = latestRecord.PFooter;
                ViewBag.copy = latestRecord.PCopyrigth;
            }
            if (string.IsNullOrEmpty(searchTerm))
            {
                var result = _context.Hotels.ToList();
                return View(result);
            }
            else
            {
                var result = _context.Hotels
                    .Where(h => h.Hotelname.Contains(searchTerm))
                    .ToList();
                return View(result);
            }
        }
        // ***********************profile user************** 
        public IActionResult Profile()
        {

            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);

            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecord != null)
            {
                ViewBag.logo = latestRecord.Imagelogo;
                ViewBag.Imagemain = latestRecord.Imagemain;
                ViewBag.pWelcom = latestRecord.PWelcome;
                ViewBag.pFooter = latestRecord.PFooter;
                ViewBag.copy = latestRecord.PCopyrigth;
            }
            
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Userrs.Where(x => x.UserId == id).SingleOrDefault();
            var userLogin = _context.Userlogins.SingleOrDefault(u => u.UserId == user.UserId);

            ViewBag.username = userLogin.Username;
            ViewBag.password = userLogin.Passwordd;

            ViewBag.Fname = user.Fname;
            ViewBag.Lname = user.Lname;
            ViewBag.Emale = user.Emale;
            ViewBag.ImagePath = user.ImagePath;
            ViewBag.Phone = user.Phone;

            return View();
        }
       
        public async Task<IActionResult> EditProfile(Userr user, string username, string password)
        {
            
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            if (user.ImageFile != null)
            {
                string wwwRootPath = _webHostEnviroment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + "_" +
               user.ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/Images/",
               fileName);
                using (var fileStream = new FileStream(path,
               FileMode.Create))
                {
                    await user.ImageFile.CopyToAsync(fileStream);
                }
                user.ImagePath = fileName;
            }
            _context.Userrs.Update(user);
            await _context.SaveChangesAsync();
            var userLogin = _context.Userlogins.SingleOrDefault(u => u.UserId == user.UserId);
            userLogin.Username = username;
            userLogin.Passwordd = password;
            _context.Userlogins.Update(userLogin);
            await _context.SaveChangesAsync();
            return RedirectToAction("Profile");


        }
        [HttpGet]
      
        public IActionResult EditProfile()
        {
            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);

            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecord != null)
            {
                ViewBag.logo = latestRecord.Imagelogo;
                ViewBag.Imagemain = latestRecord.Imagemain;
                ViewBag.pWelcom = latestRecord.PWelcome;
                ViewBag.pFooter = latestRecord.PFooter;
                ViewBag.copy = latestRecord.PCopyrigth;
            }

            var id = HttpContext.Session.GetInt32("UserId");
            if (id == null)
            {
                return RedirectToAction("Login", "Log");
            }
            else
            {
                Userr user = _context.Userrs.Where(x => x.UserId == id).SingleOrDefault();
                return View(user);
            }

            var user1 = _context.Userrs.Where(x => x.UserId == id).SingleOrDefault();
            var userLogin = _context.Userlogins.SingleOrDefault(u => u.UserId == user1.UserId);

            ViewBag.username = userLogin.Username;
            ViewBag.password = userLogin.Passwordd;


        }
        //**************************Testmonial************************
        [HttpGet]
        public IActionResult AddTestimonial()
        {
            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);

            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecord != null)
            {
                ViewBag.logo = latestRecord.Imagelogo;
                ViewBag.Imagemain = latestRecord.Imagemain;
                ViewBag.pWelcom = latestRecord.PWelcome;
                ViewBag.pFooter = latestRecord.PFooter;
                ViewBag.copy = latestRecord.PCopyrigth;
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTestimonial([Bind("Id,Msg")] Testmonial testmonial)
        {
            
            var userId = HttpContext.Session.GetInt32("UserId");

            if (ModelState.IsValid)
            {
                testmonial.Status = "Pending"; 
                testmonial.UserId = userId;
                _context.Add(testmonial);
                await _context.SaveChangesAsync();
                return RedirectToAction("UserHome", "HomePages"); 
            }
            return View(testmonial);
        }

        // GET: HomePages
        public async Task<IActionResult> Index()
        {
              return _context.HomePages != null ? 
                          View(await _context.HomePages.ToListAsync()) :
                          Problem("Entity set 'ModelContext.HomePages'  is null.");
        }
        public IActionResult GetRoomByHotelId(int id)
        {
            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);

            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecord != null)
            {
                ViewBag.logo = latestRecord.Imagelogo;
                ViewBag.Imagemain = latestRecord.Imagemain;
                ViewBag.pWelcom = latestRecord.PWelcome;
                ViewBag.pFooter = latestRecord.PFooter;
                ViewBag.copy = latestRecord.PCopyrigth;
            }
            var room = _context.Rooms.Where(p => p.HotelId == id).Where(r => r.Isavailable == "yes").ToList();
            var hotel = _context.Hotels.Where(h => h.HotelId == id).SingleOrDefault();
            ViewBag.hotelName = hotel.Hotelname;
            return View(room);
        }
        public async Task<IActionResult> UserHome()
        {
            ViewData["service"] = _context.Services.ToList();
            ViewData["events"] = _context.Events.ToList();

            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);
            var id = HttpContext.Session.GetInt32("UserId");
            var user = _context.Userrs.Where(u=>u.UserId == id).FirstOrDefault();
            ViewBag.Fname = user.Fname;
            ViewBag.Lname = user.Lname;
            if (latestRecordc != null)
            {
                ViewBag.location = latestRecordc.Location;
                ViewBag.phone = latestRecordc.Phone;
                ViewBag.email = latestRecordc.Email;
            }
            var latestRecord = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
            if (latestRecord != null)
            {
                ViewBag.logo = latestRecord.Imagelogo;
                ViewBag.Imagemain = latestRecord.Imagemain;
                ViewBag.pWelcom = latestRecord.PWelcome;
                ViewBag.pFooter = latestRecord.PFooter;
                ViewBag.copy = latestRecord.PCopyrigth;
            }

            var hotels = _context.Hotels.ToList();

            return View(hotels);
        }
        //public IActionResult Contact_Us()
        //{

        //    var latestRecordh = _context.HomePages.OrderByDescending(c => c.Id).FirstOrDefault();
        //    if (latestRecordh != null)
        //    {
        //        ViewBag.logo = latestRecordh.Imagelogo;
        //        ViewBag.Imagemain = latestRecordh.Imagemain;
        //        ViewBag.pWelcom = latestRecordh.PWelcome;
        //        ViewBag.pFooter = latestRecordh.PFooter;
        //        ViewBag.copy = latestRecordh.PCopyrigth;
        //    }
        //    var latestRecord = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();

        //    if (latestRecord != null)
        //    {
        //        ViewBag.location = latestRecord.Location;
        //        ViewBag.phone = latestRecord.Phone;
        //        ViewBag.email = latestRecord.Email;
        //    }
        //    return View();
        //}
        // GET: HomePages/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.HomePages == null)
            {
                return NotFound();
            }

            var homePage = await _context.HomePages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homePage == null)
            {
                return NotFound();
            }

            return View(homePage);
        }

        // GET: HomePages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HomePages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImageFile,PWelcome,ImageFile2,PFooter,PCopyrigth")] HomePage homePage)
        {
            if (ModelState.IsValid)
            {
                if (homePage.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" +
                   homePage.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/",
                   fileName);
                    using (var fileStream = new FileStream(path,
                   FileMode.Create))
                    {
                        await homePage.ImageFile.CopyToAsync(fileStream);
                    }
                    homePage.Imagelogo = fileName;
                }
                if (homePage.ImageFile2 != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" +
                   homePage.ImageFile2.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/",
                   fileName);
                    using (var fileStream = new FileStream(path,
                   FileMode.Create))
                    {
                        await homePage.ImageFile2.CopyToAsync(fileStream);
                    }
                    homePage.Imagemain = fileName;
                }

                _context.Add(homePage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(homePage);
        }

        // GET: HomePages/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.HomePages == null)
            {
                return NotFound();
            }

            var homePage = await _context.HomePages.FindAsync(id);
            if (homePage == null)
            {
                return NotFound();
            }
            return View(homePage);
        }

        // POST: HomePages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,ImageFile,PWelcome,ImageFile2,PFooter,PCopyrigth")] HomePage homePage)
        {
            if (id != homePage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (homePage.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                       homePage.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/",
                       fileName);
                        using (var fileStream = new FileStream(path,
                       FileMode.Create))
                        {
                            await homePage.ImageFile.CopyToAsync(fileStream);
                        }
                        homePage.Imagelogo = fileName;
                    }
                    if (homePage.ImageFile2 != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" +
                       homePage.ImageFile2.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/",
                       fileName);
                        using (var fileStream = new FileStream(path,
                       FileMode.Create))
                        {
                            await homePage.ImageFile2.CopyToAsync(fileStream);
                        }
                        homePage.Imagemain = fileName;
                    }
                    _context.Update(homePage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomePageExists(homePage.Id))
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
            return View(homePage);
        }

        // GET: HomePages/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.HomePages == null)
            {
                return NotFound();
            }

            var homePage = await _context.HomePages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (homePage == null)
            {
                return NotFound();
            }

            return View(homePage);
        }

        // POST: HomePages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.HomePages == null)
            {
                return Problem("Entity set 'ModelContext.HomePages'  is null.");
            }
            var homePage = await _context.HomePages.FindAsync(id);
            if (homePage != null)
            {
                _context.HomePages.Remove(homePage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HomePageExists(decimal id)
        {
          return (_context.HomePages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
