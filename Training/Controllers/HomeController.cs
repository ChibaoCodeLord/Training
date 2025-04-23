using System.Diagnostics;
using System.Security.Principal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Training.Data;
using Training.Models;
using Training.Service;

namespace Training.Controllers
{
    public class HomeController : Controller
    {
        Infor loginModel = new Infor();
        private readonly ILogger<HomeController> _logger;
        private readonly CourseService _courseService;
        private readonly TrainingDbContext _context;

        public HomeController(ILogger<HomeController> logger, CourseService courseService, TrainingDbContext context) // Inject CourseService via constructor
        {
            _logger = logger;
            _courseService = courseService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult IndexUser()
        {
            return View();
        }
        public IActionResult Course()
        {
            var courses = _context.Courses
        .Select(c => new CourseModel
        {
            CourseId = c.CourseId,
            CourseName = c.CourseName,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            TuitionFee = c.TuitionFee,
            MaxStudents = c.MaxStudents,
            CourseImagePath = c.CourseImagePath,
            Description = c.Description,
            Lecture = c.Lecture,
            count = _context.CourseRegistrations.Count(cr => cr.CourseId == c.CourseId)
        }).ToList();
            return View(courses);
        }

  

        // GET: CourseModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CourseModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseModel courseModel, IFormFile CourseImageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage);
                    TempData["Msg"] = "Dữ liệu không hợp lệ: " + string.Join(", ", errors);
                    return RedirectToAction(nameof(Course));
                }
                if (courseModel.StartDate > courseModel.EndDate)
                {
                    TempData["Msg"] = "Ngày bắt đầu không được lớn hơn ngày kết thúc";
                    return RedirectToAction(nameof(Course));
                }
                if (courseModel.MaxStudents <= 0)
                {
                    TempData["Msg"] = "Số lượng học viên không hợp lệ";
                    return RedirectToAction(nameof(Course));
                }
                if (courseModel.TuitionFee <= 0)
                {
                    TempData["Msg"] = "Học phí không hợp lệ";
                    return RedirectToAction(nameof(Course));
                }

                // Xử lý upload ảnh
                if (CourseImageFile != null && CourseImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine("wwwroot", "images", "courses");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}_{CourseImageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await CourseImageFile.CopyToAsync(stream);
                    }

                    courseModel.CourseImagePath = $"/images/courses/{uniqueFileName}";
                }

                var dbCourse = new Course
                {
                    CourseId = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper(),
                    CourseName = courseModel.CourseName,
                    Description = courseModel.Description,
                    StartDate = courseModel.StartDate,
                    EndDate = courseModel.EndDate,
                    TuitionFee = courseModel.TuitionFee,
                    MaxStudents = courseModel.MaxStudents,
                    Lecture = courseModel.Lecture,
                    CourseImagePath = courseModel.CourseImagePath
                };
                _context.Courses.Add(dbCourse);
             
                await _context.SaveChangesAsync();

                TempData["Msg"] = "Tạo khóa học thành công";
                return RedirectToAction(nameof(Course));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo khóa học");
                TempData["Msg"] = "Đã xảy ra lỗi khi tạo khóa học";
                return RedirectToAction(nameof(Course));
            }
        }

        // GET: CourseModels/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseModel = await _context.Courses.FindAsync(id);
            if (courseModel == null)
            {
                return NotFound();
            }
            return View(courseModel);
        }

        // POST: CourseModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CourseId,CourseName,StartDate,EndDate,TuitionFee,MaxStudents,Description,Lecture")] CourseModel courseModel, IFormFile CourseImageFile)
        {
            if (id != courseModel.CourseId)
            {
                TempData["Msg"] = "ID khóa học không hợp lệ.";
                return RedirectToAction(nameof(Course));
            }

            var dbCourse = await _context.Courses.FindAsync(id);
            if (dbCourse == null)
            {
                TempData["Msg"] = "Không tìm thấy khóa học để chỉnh sửa.";
                return RedirectToAction(nameof(Course));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                TempData["Msg"] = "Dữ liệu không hợp lệ: " + string.Join(", ", errors);
                return RedirectToAction(nameof(Course));
            }

            if (courseModel.StartDate > courseModel.EndDate) 
            {
                TempData["Msg"] = "Ngày bắt đầu không được lớn hơn ngày kết thúc.";
                return RedirectToAction(nameof(Course));
            }

            if (courseModel.MaxStudents <= 0)
            {
                TempData["Msg"] = "Số lượng học viên tối đa phải lớn hơn 0.";
                return RedirectToAction(nameof(Course));
            }

            if (courseModel.TuitionFee <= 0)
            {
                TempData["Msg"] = "Học phí phải lớn hơn 0.";
                return RedirectToAction(nameof(Course));
            }

            try
            {
                // Nếu có ảnh mới, tiến hành lưu
                if (CourseImageFile != null && CourseImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine("wwwroot", "images", "courses");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}_{CourseImageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await CourseImageFile.CopyToAsync(stream);
                    }

                    // Cập nhật ảnh mới
                    dbCourse.CourseImagePath = $"/images/courses/{uniqueFileName}";
                }
                // Nếu không có ảnh mới, giữ nguyên ảnh cũ (không cần xử lý gì thêm)

                // Cập nhật thông tin khác
                dbCourse.CourseName = courseModel.CourseName;
                dbCourse.StartDate = courseModel.StartDate;
                dbCourse.EndDate = courseModel.EndDate;
                dbCourse.TuitionFee = courseModel.TuitionFee;
                dbCourse.MaxStudents = courseModel.MaxStudents;
                dbCourse.Description = courseModel.Description;
                dbCourse.Lecture = courseModel.Lecture;

                _context.Update(dbCourse);
                await _context.SaveChangesAsync();

                TempData["Msg"] = "Cập nhật khóa học thành công.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseModelExists(courseModel.CourseId))
                {
                    TempData["Msg"] = "Không tìm thấy khóa học để chỉnh sửa.";
                    return RedirectToAction(nameof(Course));
                }
                else
                {
                    TempData["Msg"] = "Lỗi đồng thời khi cập nhật khóa học.";
                    return RedirectToAction(nameof(Course));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi chỉnh sửa khóa học.");
                TempData["Msg"] = "Đã xảy ra lỗi khi chỉnh sửa khóa học.";
            }

            return RedirectToAction(nameof(Course));
        }



        // GET: CourseModels/Delete/5
        [HttpPost]
        public async Task<JsonResult> Delete_Registered([FromBody] dynamic data)
        {
            string id = data.id;
            if (id == null)
            {
                return Json(new { success = false, message = "ID không hợp lệ." });
            }

            var studentId = HttpContext.Session.GetString("AccountId");
            var courseModel = await _context.CourseRegistrations
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.CourseId == id && m.StudentId == studentId);

            if (courseModel == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đăng ký khóa học." });
            }

            var startDate = courseModel.Course.StartDate;
            if (DateOnly.FromDateTime(DateTime.Now) >= startDate)
            {
                return Json(new { success = false, message = "Khóa học đã bắt đầu, không thể hủy đăng ký." });
            }

            _context.CourseRegistrations.Remove(courseModel);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Hủy đăng ký khóa học thành công." });
        }



        // POST: CourseModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            // Kiểm tra id hợp lệ
            if (string.IsNullOrEmpty(id))
            {
                TempData["Msg"] = "ID khóa học không hợp lệ.";
                return RedirectToAction(nameof(Course));
            }

            // Tìm khóa học
            var courseModel = await _context.Courses.FindAsync(id);
            if (courseModel == null)
            {
                TempData["Msg"] = "Không tìm thấy khóa học để xóa.";
                return RedirectToAction(nameof(Course));
            }

            // Xóa các đăng ký liên quan trong CourseRegistrations
            try
            {
                var registrations = await _context.CourseRegistrations
                    .Where(cr => cr.CourseId == id)
                    .ToListAsync();
                if (registrations.Any())
                {
                    _context.CourseRegistrations.RemoveRange(registrations);
                    await _context.SaveChangesAsync(); // Lưu thay đổi sau khi xóa CourseRegistrations
                }

                // Xóa khóa học
                _context.Courses.Remove(courseModel);
                await _context.SaveChangesAsync();
                TempData["Msg"] = "Xóa khóa học thành công.";
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa khóa học do ràng buộc khóa ngoại hoặc lỗi cơ sở dữ liệu.");
                TempData["Msg"] = "Không thể xóa khóa học do ràng buộc khóa ngoại hoặc lỗi cơ sở dữ liệu.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi xóa khóa học.");
                TempData["Msg"] = "Đã xảy ra lỗi khi xóa khóa học.";
            }

            return RedirectToAction(nameof(Course));
        }
        private bool CourseModelExists(string id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
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
                return RedirectToAction("Course");
            }

            if (student == null)
            {
                TempData["Msg"] = "Người dùng không tồn tại.";
                return RedirectToAction("Course");
            }
            int currentStudents = _context.CourseRegistrations.Count(cr => cr.CourseId == id);

            if (currentStudents==course.MaxStudents)
            {
                TempData["Msg"] = "Khóa học đã đủ số lượng học viên.";
                return RedirectToAction("Course");
            }

            var existingRegistration = _context.CourseRegistrations.FirstOrDefault(r => r.CourseId == id && r.StudentId == StudentId);
            if (existingRegistration != null)
            {
                TempData["Msg"] = "Bạn đã đăng ký khóa học này rồi.";
                return RedirectToAction("Course");
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
            return RedirectToAction("Course");
        }
    }
}
