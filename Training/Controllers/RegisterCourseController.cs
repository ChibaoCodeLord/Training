using Microsoft.AspNetCore.Mvc;
using Training.Data;
using Training.Models;
namespace Training.Controllers
{

    public class RegisterCourseController : Controller
    {
        private readonly TrainingDbContext _context;
        public RegisterCourseController(TrainingDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string id)
        {
            var course = _context.Courses.FirstOrDefault(c => c.CourseId == id);
            var StudentId = HttpContext.Session.GetString("AccountId");
            var student = _context.Students.FirstOrDefault(s => s.StudentId == StudentId);

            if (course == null)
            {
                TempData["Msg"] = "Khóa học không tồn tại.";
                return RedirectToAction("Course", "Home");
            }

            if (student == null)
            {
                TempData["Msg"] = "Người dùng không tồn tại.";
                return RedirectToAction("Course", "Home");
            }
            var existingRegistration = _context.CourseRegistrations.FirstOrDefault(r => r.CourseId == id && r.StudentId == StudentId);
            if (existingRegistration != null)
            {
                TempData["Msg"] = "Bạn đã đăng ký khóa học này rồi.";
                return RedirectToAction("Course", "Home");
            }
            int currentStudents = _context.CourseRegistrations.Count(cr => cr.CourseId == id);

            if (currentStudents == course.MaxStudents)
            {
                TempData["Msg"] = "Khóa học đã đủ số lượng học viên.";
                return RedirectToAction("Course", "Home");
            }

            
            var courseRegistration = new CourseRegistration
            {
                RegistrationId = Guid.NewGuid().ToString(),
                CourseId = id,
                StudentId = StudentId,
                RegistrationDate = DateTime.Now,
                Status_Register = "Đang chờ duyệt"
            };

            _context.CourseRegistrations.Add(courseRegistration);
            _context.SaveChanges();

            TempData["Msg"] = "Đăng ký khóa học thành công!";
            return RedirectToAction("Course", "Home");
        }
    }
}
