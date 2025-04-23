using System;
using System.Collections.Generic;

namespace Training.Data;

public partial class CourseRegistration
{
    public string RegistrationId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public DateTime? RegistrationDate { get; set; }

   public string Status_Register { get; set; } = null!;

    public string? PaymentStatus { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
