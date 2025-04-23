using Microsoft.EntityFrameworkCore;
using Training.Data;
using Training.Models;
namespace Training.Service
{
    public class CourseServiceImpl : CourseService
    {
        private TrainingDbContext db;
        public CourseServiceImpl(TrainingDbContext db)
        {
            this.db = db;
        }
        public bool Create(CourseModel course)
        {
            using var transaction = db.Database.BeginTransaction();
            try
            {
               
                if (db.Courses.Any(c => c.CourseName == course.CourseName))
                    throw new Exception("Tên khóa học đã tồn tại");
                            
                var dbCourse = new Course
                {
                    CourseId = Guid.NewGuid().ToString("N").Substring(0, 5).ToUpper(),
                    CourseName = course.CourseName,
                    Description = course.Description,
                    StartDate = course.StartDate,
                    EndDate = course.EndDate,
                    TuitionFee = course.TuitionFee,
                    MaxStudents = course.MaxStudents,
                    Lecture = course.Lecture,
                    CourseImagePath = course.CourseImagePath
                };
                db.Courses.Add(dbCourse);
                db.SaveChanges();
                transaction.Commit();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database Error: {dbEx.InnerException?.Message}");
                throw new Exception("Lỗi khi lưu dữ liệu: " + dbEx.InnerException?.Message);
            }
        }
    }
}
