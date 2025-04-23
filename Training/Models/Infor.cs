using System.ComponentModel.DataAnnotations;
using Training.Data;

namespace Training.Models
{
    public class Infor
    {
        public string? AccountId { get; set; }

        [Required(ErrorMessage = "Username không được để trống")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password không được để trống")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string? ConfirmPassword { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]

        public string? Role { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string? FullName { get; set; }


        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        public DateTime? DateOfBirth { get; set; }


        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        public string? PhoneNumber { get; set; }


        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }



    }
}
