using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class Progress
{
    public int ProgressId { get; set; }

    public int? UserId { get; set; }

    public int? LessonId { get; set; }

    public bool? Completed { get; set; }

    public double? Score { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Lesson? Lesson { get; set; }

    public virtual User? User { get; set; }
}
