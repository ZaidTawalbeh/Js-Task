using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Composition;
using TravelMate.Models;
using System.Threading.Tasks;

namespace TravelMate.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public AdminController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Index()
        {
            var id = HttpContext.Session.GetInt32("AdminId");
            var user = _context.Userrs.Where(x => x.UserId == id).SingleOrDefault();

            ViewBag.Fname = user.Fname;
            ViewBag.Lname = user.Lname;
            ViewBag.Emale = user.Emale;
            ViewBag.ImagePath = user.ImagePath;
            ViewBag.Phone = user.Phone;

            ViewData["objOfuser"] = _context.Userrs.ToList();
            ViewBag.numOfregUser = _context.Userrs.Count();
            ViewBag.availableRooms = _context.Rooms.Where(x => x.Isavailable == "yes").Count();
            ViewBag.bookedRooms = _context.Rooms.Where(x => x.Isavailable == "no").Count();
            var modelcontext = _context.Reservations.Include(x => x.User).Include(x => x.Room).ToList();
            ViewBag.TotalMoney = modelcontext.Sum(x => x.Room.Pricepernight * (x.Checkoutdate.Value.Date - x.Checkindate.Value.Date).Days);

            return View(user);
        }

        //*********************************JoinTabel*****************************//
        public IActionResult JoinTabel()
        {
            var reservations = _context.Reservations.ToList();
            var user = _context.Userrs.ToList();
            var room = _context.Rooms.ToList();
            var hotel = _context.Hotels.ToList();
            var result = from u in user
                         join re in reservations on u.UserId equals re.UserId
                         join ro in room on re.RoomId equals ro.RoomId
                         join h in hotel on ro.HotelId equals h.HotelId
                         select new JoinTabel
                         {
                             Reservation = re,
                             User = u,
                             Room = ro,
                             Hotel = h
                         };

            return View(result);
        }

        //*******************************Search********************************//
        [HttpGet]
        public IActionResult Search()
        {
            var modelcontext = _context.Reservations.Include(x => x.User).Include(x =>x.Room).ToList();
            return View(modelcontext);
        }
        public IActionResult Search(DateTime? startDate, DateTime? endDate)
        {
            var modelcontext = _context.Reservations.Include(x => x.User).Include(x => x.Room).ToList();
            ViewBag.TotalPrieeee = modelcontext.Sum(x => x.Room.Pricepernight * (x.Checkoutdate.Value.Date - x.Checkindate.Value.Date).Days);

            if (startDate == null && endDate == null)
            {
                ViewBag.TotalPrie = modelcontext.Sum(x => x.Room.Pricepernight * (x.Checkoutdate.Value.Date - x.Checkindate.Value.Date).Days);
                return View(modelcontext);
            }
            else if (startDate != null && endDate == null)
            {
                modelcontext = modelcontext.Where(x => x.Checkindate.Value.Date >= startDate).ToList();
                ViewBag.TotalPrie = modelcontext.Sum(x => x.Room.Pricepernight * (x.Checkoutdate.Value.Date - x.Checkindate.Value.Date).Days);

                return View(modelcontext);
            }
            else if (startDate == null && endDate != null)
            {
                modelcontext = modelcontext.Where(x => x.Checkindate.Value.Date <= endDate).ToList();
                ViewBag.TotalPrie = modelcontext.Sum(x => x.Room.Pricepernight * (x.Checkoutdate.Value.Date - x.Checkindate.Value.Date).Days);
                return View(modelcontext);
            }
            else
            {
                modelcontext = modelcontext.Where(x => x.Checkindate.Value.Date >= startDate && x.Checkindate.Value.Date <= endDate).ToList();
                ViewBag.TotalPrie = modelcontext.Sum(x => x.Room.Pricepernight * (x.Checkoutdate.Value.Date - x.Checkindate.Value.Date).Days);
                return View(modelcontext);
            }
        }
        //***********************************Report*******************************//
        [HttpGet]
        public IActionResult Report()
        {

            var modelcontext = _context.Reservations.Include(x => x.User).Include(x => x.Room).ToList();
            var reservations = _context.Reservations.ToList();
            var user = _context.Userrs.ToList();
            var room = _context.Rooms.ToList();
            var hotel = _context.Hotels.ToList();
            var result = from u in user
                         join re in reservations on u.UserId equals re.UserId
                         join ro in room on re.RoomId equals ro.RoomId
                         join h in hotel on ro.HotelId equals h.HotelId
                         select new JoinTabel
                         {
                             Reservation = re,
                             User = u,
                             Room = ro,
                             Hotel = h
                         };

            var model3 = Tuple.Create<IEnumerable<JoinTabel>, IEnumerable<Reservation>>(result, modelcontext);
            ViewBag.TotalPrieeee = modelcontext.Sum(x => x.Room.Pricepernight * (x.Checkoutdate.Value.Date - x.Checkindate.Value.Date).Days);

            return View(model3);
        }
        [HttpPost]
        public async Task<IActionResult> Report(string reportType, DateTime? startDate, DateTime? endDate, string month, int? year)
        {
            ViewBag.numOfregUser = _context.Reservations.Count();
            ViewBag.availableRooms = _context.Rooms.Where(x => x.Isavailable == "yes").Count();
            ViewBag.bookedRooms = _context.Rooms.Where(x => x.Isavailable == "no").Count();
            var total = _context.Reservations.Include(x => x.User).Include(x => x.Room).ToList();
            ViewBag.TotalMoney = total.Sum(x => x.Room.Pricepernight * (x.Checkoutdate.Value.Date - x.Checkindate.Value.Date).Days);

            var reservations = _context.Reservations.ToList();
            var user = _context.Userrs.ToList();
            var room = _context.Rooms.ToList();
            var hotel = _context.Hotels.ToList();
            var result = from u in user
                         join re in reservations on u.UserId equals re.UserId
                         join ro in room on re.RoomId equals ro.RoomId
                         join h in hotel on ro.HotelId equals h.HotelId
                         select new JoinTabel
                         {
                             Reservation = re,
                             User = u,
                             Room = ro,
                             Hotel = h
                         };

            var modelcontext = _context.Reservations.Include(x => x.User).Include(x => x.Room).ToList();
            IEnumerable<Reservation> filteredReservations = modelcontext;

            if (reportType == "date" && startDate.HasValue && endDate.HasValue)
            {
                filteredReservations = filteredReservations.Where(x => x.Checkindate.Value.Date >= startDate && x.Checkindate.Value.Date <= endDate).ToList();
            }
            else if (reportType == "month" && !string.IsNullOrEmpty(month))
            {
                DateTime selectedMonth = DateTime.ParseExact(month + "-01", "yyyy-MM-dd", null);
                filteredReservations = filteredReservations.Where(x => x.Checkindate.Value.Year == selectedMonth.Year && x.Checkindate.Value.Month == selectedMonth.Month).ToList();
            }
            else if (reportType == "year" && year.HasValue)
            {
                filteredReservations = filteredReservations.Where(x => x.Checkindate.Value.Year == year).ToList();
            }

            var model3 = Tuple.Create(result, filteredReservations);
            ViewBag.TotalPrie = filteredReservations.Sum(x => x.Room.Pricepernight * (x.Checkoutdate.Value.Date - x.Checkindate.Value.Date).Days);

            return View(model3);
        }

       //************************Profile*********************************//
        public IActionResult Profile()
        {
            ViewBag.username = HttpContext.Session.GetString("AdminName");
            var id = HttpContext.Session.GetInt32("AdminId");
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
        [HttpPost]
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
            var id = HttpContext.Session.GetInt32("AdminId");
            if (id == null)
            {
                return RedirectToAction ("Login","Log");
            }
            else
            {
                Userr user = _context.Userrs.Where(x => x.UserId == id).SingleOrDefault();
                return View(user);
            }
        }
        //***************************** testmonail******************************//
		public async Task<IActionResult> Testmonails()
		{
			var pendingTestimonials = await _context.Testmonials.Where(t => t.Status == "Pending").Include(x=>x.User).ToListAsync();
			return View(pendingTestimonials);
		}

        [HttpPost]
        public async Task<IActionResult> ApproveTestimonial(decimal id)
        {
            var testimonial = await _context.Testmonials.FindAsync(id);
            if (testimonial != null)
            {
                testimonial.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Testmonails));
        }

        [HttpPost]
        public async Task<IActionResult> RejectTestimonial(decimal id)
        {
            var testimonial = await _context.Testmonials.FindAsync(id);
            if (testimonial != null)
            {
                testimonial.Status = "Rejected";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Testmonails));
        }
    }
}
