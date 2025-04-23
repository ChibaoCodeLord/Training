using Training.Data;
namespace Training.Models
{
    public class Course_RegisteredModel
    {
        public string RegistrationId { get; set; } = null!;
        public string StudentId { get; set; } = null!;
        public string CourseId { get; set; } = null!;
        public string NameCourse { get; set; } = null!;
        public DateTime? RegistrationDate { get; set; }
        public string Status_Register { get; set; } = null!;
        public string? PaymentStatus { get; set; }
        public string Lecture { get; set; } = null!;
        public virtual Course Course { get; set; } = null!;
        public virtual CourseRegistration CourseRegistration { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
        public int count { get; set; }
    }
}
