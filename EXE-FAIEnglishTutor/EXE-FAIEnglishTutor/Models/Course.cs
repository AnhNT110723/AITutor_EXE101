using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class Course
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = null!;

    public string? Description { get; set; }

    public int Duration { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? TypeId { get; set; }

    public virtual ICollection<Feeback> Feebacks { get; set; } = new List<Feeback>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Type? Type { get; set; }
}
