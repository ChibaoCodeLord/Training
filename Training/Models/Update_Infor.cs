namespace Training.Models
{
    public class Update_Infor
    {
        public string StudentId { get; set; }
        public string FullName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public IFormFile? ProfileImage { get; set; }
        public string CurrentProfileImage { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }

}
