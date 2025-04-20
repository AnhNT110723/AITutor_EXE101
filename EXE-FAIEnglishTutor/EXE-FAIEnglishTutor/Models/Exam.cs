using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class Exam
{
    public int ExamId { get; set; }

    public int? ExamTypeId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ExamSection> ExamSections { get; set; } = new List<ExamSection>();

    public virtual ExamType? ExamType { get; set; }

    public virtual ICollection<UserExamResult> UserExamResults { get; set; } = new List<UserExamResult>();
}
