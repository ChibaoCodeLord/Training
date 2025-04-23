using Training.Data;

namespace Training.Models
{
    public class RegisterModel
    {
        public string RegistrationId { get; set; } = null!;

        public string StudentId { get; set; } = null!;

        public string CourseId { get; set; } = null!;
        public string NameCourse { get; set; } = null!;

        public DateTime? RegistrationDate =DateTime.Now;

        public string Status_Register { get; set; } = null!;

        public string? PaymentStatus { get; set; }

        public virtual Course Course { get; set; } = null!;

        public virtual Student Student { get; set; } = null!;


    }
}
