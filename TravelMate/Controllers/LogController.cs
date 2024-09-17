using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelMate.Models;
using System.Net;
using System.Net.Mail;
using iTextSharp.text.pdf;


namespace TravelMate.Controllers
{
    public class LogController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public LogController (ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("UserId,Fname,Lname,Phone,Emale,ImageFile")] Userr userr , string userName , string password)
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

                Userlogin userlogin = new Userlogin();
                userlogin.Username = userName;
                userlogin.Passwordd = password;
                userlogin.UserId = userr.UserId;
                userlogin.RoleId = 2;
                _context.Add(userlogin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Login));
            }
            return View(userr);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Id,Username,Passwordd,UserId,RoleId")] Userlogin userlogin)
        {
            var auth = _context.Userlogins.Where(x => x.Username == userlogin.Username && x.Passwordd == userlogin.Passwordd).FirstOrDefault();
            if (auth != null)
            {
                //1 = Admin
                //2 = Customer
                //3 = Employee
                switch (auth.RoleId)
                {
                    case 1:
                        HttpContext.Session.SetInt32("AdminId", (int)auth.UserId);
                        HttpContext.Session.SetString("AdminName", auth.Username);
                        return RedirectToAction("Index", "Admin");

                    case 2:
                        HttpContext.Session.SetInt32("UserId", (int)auth.UserId);
                        return RedirectToAction("UserHome", "HomePages");
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult ForgotPassword()
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Userrs.FirstOrDefault(u => u.Emale== model.Email);
                if (user != null)
                {
                    string resetToken = Guid.NewGuid().ToString();

                    HttpContext.Session.SetString("ResetToken", resetToken);
                    HttpContext.Session.SetString("ResetEmail", model.Email);

                    string resetLink = Url.Action("ResetPassword", "Log", new { token = resetToken }, Request.Scheme);

                    SendEmail(model.Email, resetLink);
                }

                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }
        public IActionResult ForgotPasswordConfirmation()
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
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
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
            var sessionToken = HttpContext.Session.GetString("ResetToken");
            if (sessionToken == null || sessionToken != token)
            {
                return RedirectToAction("Error"); 
            }

            return View(); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            
                var email = HttpContext.Session.GetString("ResetEmail");
                var user = _context.Userrs.FirstOrDefault(u => u.Emale == email);
                var id = user.UserId;
                var userlog = _context.Userlogins.Where(x=>x.UserId == id).FirstOrDefault();

                if (user != null)
                {
                    userlog.Passwordd = model.NewPassword;
                    _context.Update(userlog);
                    _context.SaveChanges();

                    HttpContext.Session.Remove("ResetToken");
                    HttpContext.Session.Remove("ResetEmail");

                    return RedirectToAction("Login");
                }
            
            return View(model);
        }
        private void SendEmail(string email, string resetLink)
        {
            var latestRecord = _context.ContactPages.OrderByDescending(c => c.Id).FirstOrDefault();

            var myemail = latestRecord.Email;
       
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(myemail);
                mail.To.Add(email);
                mail.Subject = "Password Reset Link";
                mail.Body = $"Please reset your password by clicking on the following link: {resetLink}";
                mail.IsBodyHtml = true;  


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
    }
}
