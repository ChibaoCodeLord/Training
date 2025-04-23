using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Training.Data;
using Training.Models;

namespace Training.Controllers
{
    public class ManageController:Controller
    {
        public readonly TrainingDbContext _context;
        public ManageController(TrainingDbContext context)
        {
            _context = context;
        }
        public IActionResult Manage_User() {
            return View();
        }
        public IActionResult Manage_Admin()
        {
            return View();
        }

        public IActionResult Manage_Course()
        {
            return View();
        }
        public IActionResult Student_Manage()
        {
            var registrations = _context.CourseRegistrations
           .Include(r => r.Student)
           .Include(r => r.Course)
           .ToList();

            return View(registrations);
           
        }
        [HttpGet]
        public IActionResult My_Course()
        {
            var studentId = HttpContext.Session.GetString("AccountId");
            var student = _context.Students.FirstOrDefault(s => s.StudentId == studentId);
            if (student == null) {
                TempData["Msg"] = "Người dùng không tồn tại.";
                return RedirectToAction("Course", "Home");
            }

            var result = (from r in _context.CourseRegistrations
                          join c in _context.Courses on r.CourseId equals c.CourseId
                          where r.StudentId == studentId
                          select new Course_RegisteredModel
                          {
                              CourseRegistration = r,
                              Course = c
                          }).ToList();
            return View(result);
        }
        public ActionResult CourseStatistics()
        {
            // Lấy toàn bộ dữ liệu đăng ký khóa học
            var data = _context.CourseRegistrations
                .Select(r => new Course_RegisteredModel
                {
                    RegistrationId = r.RegistrationId,
                    StudentId = r.StudentId,
                    CourseId = r.CourseId,
                    NameCourse = r.Course.CourseName,
                    RegistrationDate = r.RegistrationDate,
                    Status_Register = r.Status_Register,
                    PaymentStatus = r.PaymentStatus,
                    Lecture = r.Course.Lecture,
                    Course = r.Course,
                    CourseRegistration = r,
                    Student = r.Student
                }).ToList();

            return View(data);
        }

        public ActionResult CourseStatisticsView()
        {
            return View();
        }
       

    public ActionResult Dashboard()
        {
            var courseStats = _context.CourseRegistrations
                                .GroupBy(r => r.Course.CourseName)
                                .Select(g => new Course_RegisteredModel
                                {
                                    NameCourse = g.Key,
                                    count = g.Select(r => r.StudentId).Distinct().Count()
                                }).ToList();

            return View(courseStats);
        }
        public ActionResult CourseStatics()
        {
            var data = _context.CourseRegistrations
                .Include(x => x.Course) 
                .Include(x=>x.Course.TuitionFee)
                .Where(x => x.RegistrationDate.HasValue && x.RegistrationDate.Value.Year == 2025)
                .ToList();

            return View(data);
        }
        public IActionResult Account_Manage()
        {
            var accounts = _context.Accounts
                .Include(a => a.Student)
                .ToList();
            return View(accounts);
        }
        public IActionResult Create(Training.Data.Account  account)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {

                if (_context.Students.Any(s => s.Email == account.Student.Email))
                    throw new Exception("Email đã tồn tại");
                if (_context.Accounts.Any(a => a.Username == account.Username))
                    throw new Exception("Username đã tồn tại");
                if (_context.Students.Any(s => s.PhoneNumber == account.Student.PhoneNumber))
                    throw new Exception("Số điện thoại đã tồn tại");


                var dbAccount = new Account
                {
                    AccountId = Guid.NewGuid().ToString("N").Substring(0, 5),
                    Username = account.Username,
                    Password = account.Password,
                    Role = account.Role ?? "Student"
                };
                _context.Accounts.Add(dbAccount);


                var dbStudent = new Student
                {
                    StudentId = dbAccount.AccountId,
                    FullName = account.Student.FullName,
                    Email = account.Student.Email,
                    PhoneNumber = account.Student.PhoneNumber,
                    DateOfBirth = account.Student.DateOfBirth,
                };
                _context.Students.Add(dbStudent);

                _context.SaveChanges();
                TempData["Msg"] = "Tạo tài khoản thành công !";
                transaction.Commit();
                return View("Account_Manage", "Manage");
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Msg"] = "Tạo tài khoản thất bại !"; 
                Console.WriteLine($"Database Error: {dbEx.InnerException?.Message}");
                throw new Exception("Lỗi khi lưu dữ liệu: " + dbEx.InnerException?.Message);
            }
        }
    }
}
