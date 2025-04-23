using System;
using System.Collections.Generic;

namespace Training.Data;

public partial class Course
{
    public string CourseId { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal TuitionFee { get; set; }

    public int MaxStudents { get; set; }

    public string? CourseImagePath { get; set; }

    public string? Description { get; set; }
    public string? Lecture { get; set; }

    public virtual ICollection<CourseRegistration> CourseRegistrations { get; set; } = new List<CourseRegistration>();
}
