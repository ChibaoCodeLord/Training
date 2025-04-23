using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Training.Models
{
    public class CourseModel
    {
        [Key]
        public string CourseId { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "Tên khóa học là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khóa học không quá 100 ký tự")]
        public string CourseName { get; set; } = null!;

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Học phí là bắt buộc")]
        [Range(0, 100000000, ErrorMessage = "Học phí phải từ 0 đến 100,000,000")]
        public decimal TuitionFee { get; set; }

        [Required(ErrorMessage = "Số lượng học viên là bắt buộc")]
        [Range(1, 1000, ErrorMessage = "Số lượng học viên phải từ 1 đến 1000")]
        public int MaxStudents { get; set; }


        [NotMapped]
        public IFormFile? CourseImageFile { get; set; }
        public string? CourseImagePath { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không quá 500 ký tự")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Tên giảng viên không quá 100 ký tự")]
        public string? Lecture { get; set; }
        public int count { get; set; } = 0;
    }
}