using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class ExamType
{
    public int ExamTypeId { get; set; }

    public string? ExamName { get; set; }

    public virtual ICollection<ExamPart> ExamParts { get; set; } = new List<ExamPart>();

    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
