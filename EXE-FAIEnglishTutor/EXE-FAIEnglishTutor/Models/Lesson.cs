using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class Lesson
{
    public int LessonId { get; set; }

    public int? CourseId { get; set; }

    public string Title { get; set; } = null!;

    public string Situation { get; set; } = null!;

    public string? VideoUrl { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
}
