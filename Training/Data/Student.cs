using System;
using System.Collections.Generic;

namespace Training.Data;

public partial class Student
{
    public string StudentId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string? PhoneNumber { get; set; }

    public string Email { get; set; } = null!;

    public string? ProfileImagePath { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<CourseRegistration> CourseRegistrations { get; set; } = new List<CourseRegistration>();

    public virtual Account StudentNavigation { get; set; } = null!;
}
