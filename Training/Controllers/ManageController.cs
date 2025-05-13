using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Training.Data;
using Training.Models;
using Training.Send_Email;

namespace Training.Controllers
{
    public class ManageController:Controller
    {
        public readonly TrainingDbContext _context;
        public ManageController(TrainingDbContext context)
        {
            _context = context;
        }
        public IActionResult Update_Infor()
        {
            var model = new Update_Infor();
            return View(model);
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
        //Doanh thu ne
        public ActionResult CourseStatistics()
        {
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

        //TK Số lượng 
        public ActionResult Dashboard()
        {
            var allCourses = _context.Courses.ToList();

            var registrationCounts = _context.CourseRegistrations
                                        .GroupBy(r => r.CourseId)
                                        .Select(g => new
                                        {
                                            CourseId = g.Key,
                                            Count = g.Select(r => r.StudentId).Distinct().Count()
                                        }).ToDictionary(x => x.CourseId, x => x.Count);

            var courseStats = allCourses.Select(course => new Course_RegisteredModel
            {
                NameCourse = course.CourseName,
                count = registrationCounts.ContainsKey(course.CourseId) ? registrationCounts[course.CourseId] : 0
            }).ToList();

            return View(courseStats);
        }
       
        public ActionResult CourseStatics()
        {
            var data = _context.CourseRegistrations
                .Include(x => x.Course) 
                .Include(x=>x.Course.TuitionFee)
                .Where(x => x.RegistrationDate.HasValue)
                .ToList();

            return View(data);
        }
        public IActionResult Account_Manage()
        {
            var accounts = _context.Accounts
                .Include(a => a.Student)
                .Where(a => a.Role == "Student")
                .ToList();
            return View(accounts);
        }
        [HttpPost]
        public IActionResult Create(Account account)
        {
            if (account == null || account.Student == null)
            {
                TempData["Msg"] = "Dữ liệu không hợp lệ";
                return RedirectToAction("Account_Manage");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
              
                if (_context.Students.Any(s => s.Email.ToLower() == account.Student.Email.ToLower()))
                {
                    TempData["Msg"] = "Email đã tồn tại";
                    return RedirectToAction("Account_Manage");
                }

                if (_context.Accounts.Any(a => a.Username.ToLower() == account.Username.ToLower()))
                {
                    TempData["Msg"] = "Username đã tồn tại";
                    return RedirectToAction("Account_Manage");
                }

                if (!string.IsNullOrEmpty(account.Student.PhoneNumber) &&
                    _context.Students.Any(s => s.PhoneNumber == account.Student.PhoneNumber))
                {
                    TempData["Msg"] = "Số điện thoại đã tồn tại";
                    return RedirectToAction("Account_Manage");
                }

               
                var dbAccount = new Account
                {
                    AccountId = Guid.NewGuid().ToString("N").Substring(0, 8),
                    Username = account.Username,
                    Password = account.Password,
                    Role = "Student"
                };
                _context.Accounts.Add(dbAccount);

                var dbStudent = new Student
                {
                    StudentId = dbAccount.AccountId,
                    FullName = account.Student.FullName,
                    Email = account.Student.Email,
                    PhoneNumber = account.Student.PhoneNumber,
                    DateOfBirth = account.Student.DateOfBirth,
                    Address = account.Student.Address
                };
                _context.Students.Add(dbStudent);

                _context.SaveChanges();
                transaction.Commit();

                //bool emailSent = SendEmail.SendAccountInfoEmail(
                //    account.Student.Email,
                //    account.Username,
                //    account.Password
                //);
                //bool result = SendEmail.SendAccountInfoEmail("recipient@example.com", "Test Username", "Test Password");
                //Console.WriteLine($"Email sent successfully: {result}");

                //if (!emailSent)
                //{
                //    TempData["Msg"] = "Thêm học sinh thành công nhưng gửi email thất bại!";
                //}
                //else
                //{
                //    TempData["Msg"] = "Thêm học sinh thành công! Thông tin đã được gửi qua email.";
                //}
                TempData["Msg"] = "Thêm học sinh thành công";
                return RedirectToAction("Account_Manage");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                TempData["Msg"] = $"Lỗi khi thêm học sinh: {ex.Message}";
                return RedirectToAction("Account_Manage");
            }
        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Msg"] = "ID học sinh không hợp lệ";
                return RedirectToAction("Account_Manage");
            }

            var account = _context.Accounts
                .Include(a => a.Student)
                .FirstOrDefault(a => a.AccountId == id);

            if (account == null)
            {
                TempData["Msg"] = "Không tìm thấy học sinh";
                return RedirectToAction("Account_Manage");
            }

            return View("Account_Manage", _context.Accounts.Include(a => a.Student).ToList());
        }

        [HttpPost]
        public IActionResult Edit(Account account)
        {
            if (account == null || account.Student == null)
            {
                TempData["Msg"] = "Dữ liệu không hợp lệ";
                return RedirectToAction("Account_Manage");
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var existingAccount = _context.Accounts
                    .Include(a => a.Student)
                    .FirstOrDefault(a => a.AccountId == account.AccountId);

                if (existingAccount == null)
                {
                    TempData["Msg"] = "Không tìm thấy học sinh";
                    return RedirectToAction("Account_Manage");
                }

                if (_context.Students.Any(s => s.Email.ToLower() == account.Student.Email.ToLower() &&
                                             s.StudentId != account.AccountId))
                {
                    TempData["Msg"] = "Email đã tồn tại";
                    return RedirectToAction("Account_Manage");
                }

                if (_context.Accounts.Any(a => a.Username.ToLower() == account.Username.ToLower() &&
                                             a.AccountId != account.AccountId))
                {
                    TempData["Msg"] = "Username đã tồn tại";
                    return RedirectToAction("Account_Manage");
                }

                if (!string.IsNullOrEmpty(account.Student.PhoneNumber) &&
                    _context.Students.Any(s => s.PhoneNumber == account.Student.PhoneNumber &&
                                             s.StudentId != account.AccountId))
                {
                    TempData["Msg"] = "Số điện thoại đã tồn tại";
                    return RedirectToAction("Account_Manage");
                }


                existingAccount.Username = account.Username;
                existingAccount.Password = account.Password;

                if (existingAccount.Student != null)
                {
                    existingAccount.Student.FullName = account.Student.FullName;
                    existingAccount.Student.DateOfBirth = account.Student.DateOfBirth;
                    existingAccount.Student.PhoneNumber = account.Student.PhoneNumber;
                    existingAccount.Student.Email = account.Student.Email;
                    existingAccount.Student.Address = account.Student.Address;
                }

                _context.SaveChanges();
                transaction.Commit();

                TempData["Msg"] = "Cập nhật học sinh thành công!";
                return RedirectToAction("Account_Manage");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                TempData["Msg"] = $"Lỗi khi cập nhật học sinh: {ex.Message}";
                return RedirectToAction("Account_Manage");
            }
        }

        [HttpPost]
        public IActionResult Delete([FromBody] string id)
        {
           
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID học sinh không hợp lệ" });
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var hasRegistrations = _context.CourseRegistrations.Any(r => r.StudentId == id);
                if (hasRegistrations)
                {
                    return Json(new { success = false, message = "Học viên đã đăng kí khóa học, không thể xóa" });
                }

                var account = _context.Accounts
                    .Include(a => a.Student)
                    .FirstOrDefault(a => a.AccountId == id);

                if (account == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy học sinh" });
                }

                
                var registrations = _context.CourseRegistrations
                    .Where(r => r.StudentId == id)
                    .ToList();

                if (registrations.Any())
                {
                    _context.CourseRegistrations.RemoveRange(registrations);
                }

           
                if (account.Student != null)
                {
                    _context.Students.Remove(account.Student);
                }

      
                _context.Accounts.Remove(account);

                _context.SaveChanges();
                transaction.Commit();

                return Json(new { success = true, message = "Xóa học sinh thành công" });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json(new { success = false, message = $"Lỗi khi xóa học sinh: {ex.Message}" });
            }
        }
        [HttpGet]
        public IActionResult Account()
        {
            var studentId = HttpContext.Session.GetString("AccountId"   );
            Console.WriteLine("Session ID: " + studentId); 

            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Msg"] = "Không tìm thấy tài khoản!";
                return View("Update_Infor", new Update_Infor());
            }


            var student = _context.Students
                                  .Include(s => s.StudentNavigation)
                                  .FirstOrDefault(s => s.StudentId == studentId); 

            if (student == null)
            {
                TempData["Msg"] = "Không tìm thấy tài khoản!";
                return View("Update_Infor", new Update_Infor());
            }

            var model = new Update_Infor
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber,
                Email = student.Email,
                Address = student.Address,
                CurrentProfileImage = student.ProfileImagePath
            };

            return View("Update_Infor", model);
        }



        [HttpPost]
        public IActionResult Account(Update_Infor model)
        {
            var studentId = HttpContext.Session.GetString("AccountId");
            var student = _context.Students
                                  .Include(s => s.StudentNavigation)
                                  .FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
            {
                TempData["Msg"] = "Không tìm thấy tài khoản!";
                return RedirectToAction("Update_Infor", "Manage");
            }

            student.FullName = model.FullName;
            student.DateOfBirth = model.DateOfBirth;
            student.PhoneNumber = model.PhoneNumber;
            student.Email = model.Email;
            student.Address = model.Address;

    
            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var fileName = Path.GetFileName(model.ProfileImage.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(stream);
                }

                student.ProfileImagePath = "/images/" + fileName;
            }
            else
            {
                student.ProfileImagePath = model.CurrentProfileImage;
            }
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (model.CurrentPassword == student.StudentNavigation.Password)
                {
                    student.StudentNavigation.Password = model.NewPassword;
                }
                else
                {
                    TempData["Msg"] = "Mật khẩu hiện tại không đúng!";
                    return RedirectToAction("Update_Infor", "Manage");
                }
            }

            _context.SaveChanges();
            TempData["Msg"] = "Cập nhật thành công!";
            return RedirectToAction("Update_Infor", "Manage");
        }


    }
}

