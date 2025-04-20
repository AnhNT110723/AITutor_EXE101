using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class ExamSection
{
    public int SectionId { get; set; }

    public int? ExamId { get; set; }

    public int? PartId { get; set; }

    public virtual Exam? Exam { get; set; }

    public virtual ExamPart? Part { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
}
