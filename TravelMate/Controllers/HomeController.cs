using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TravelMate.Models;
using System.Net.Mail;
using System.Text;
using System.Net;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.DirectoryServices;


namespace TravelMate.Controllers

{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ModelContext _context;
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(ILogger<HomeController> logger, ModelContext context, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _context = context;
            _clientFactory = clientFactory;
        }

        public IActionResult Index()
        {

            var latestRecordc = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            HttpContext.Session.SetString("Phone", latestRecordc.Phone);
            var latestRecorda = _context.AboutPages.OrderByDescending(c => c.Id).FirstOrDefault();

            if (latestRecorda != null)
            {
                ViewBag.Image = latestRecorda.Imagemain;
                ViewBag.pAbout = latestRecorda.PAbout;
            }
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
            var Testmonials = _context.Testmonials.Where(x => x.Status == "Approved").Include(_ => _.User).ToList();
            ViewBag.Testmonial = Testmonials;
            ViewData["service"] = _context.Services.ToList();
            var eventOfHotel = _context.Events.Include(x => x.Hotel).ToList();
            ViewBag.events = eventOfHotel;

            var hotels = _context.Hotels.ToList();

            return View(hotels);
        }

        public IActionResult GetRoomByHotelId(int id)
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
            var room = _context.Rooms.Where(p => p.HotelId == id).Where(r=>r.Isavailable == "yes").ToList();
            var hotel = _context.Hotels.Where(h => h.HotelId == id).SingleOrDefault();
            ViewBag.hotelName = hotel.Hotelname;

            return View(room);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

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
        [HttpGet]
        public IActionResult BookNow(int roomId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction(nameof(AskToLog));
            }

            var reservation = new Reservation
            {
                RoomId = roomId,
                UserId = userId.Value
            };

            return View(reservation);   
        }
        public IActionResult AskToLog()
        {
            return View();
        }

      

        public async Task<IActionResult> SendMessage(string name, string email, string subject, string message)
        {
            var latestRecord = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();
            var myemail = latestRecord.Email;

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(email);
                mail.To.Add(myemail);
                mail.Subject = subject;
                mail.Body = $"Name: {name}\nEmail: {email}\nSubject: {subject}\nMessage: {message}";
                mail.IsBodyHtml = false;



                using (SmtpClient smtp = new SmtpClient("smtp.office365.com", 587))
                {
                    smtp.Credentials = new NetworkCredential(email, "Zaid1996");
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;


                    try
                    {
                        await smtp.SendMailAsync(mail);

                        using (MailMessage replyMail = new MailMessage())
                        {
                            replyMail.From = new MailAddress(myemail);
                            replyMail.To.Add(email);
                            replyMail.Subject = "Thank you for contacting us - your inquiry is under follow-up";
                            replyMail.Body = "Thank you for contacting us. Our dedicated support team will respond to your inquiry as soon as possible.";
                            replyMail.IsBodyHtml = false;
                            using (SmtpClient smtpreply = new SmtpClient("smtp.office365.com", 587))
                            {
                                smtpreply.Credentials = new NetworkCredential(myemail, "Zaid1996");
                                smtpreply.EnableSsl = true;
                                smtpreply.UseDefaultCredentials = false;
                                smtpreply.DeliveryMethod = SmtpDeliveryMethod.Network;
                                await smtpreply.SendMailAsync(replyMail);
                            }
                        }

                        return RedirectToAction("Index");

                    }
                    catch (Exception ex)
                    {

                        ModelState.AddModelError("", "Error sending email: " + ex.Message);
                        return RedirectToAction("AskToLog");
                    }

                }
            }
        }

      
    }
}

