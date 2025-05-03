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

    public int? ParentExamId { get; set; }

    public string? Slug { get; set; }

    public int? Duration { get; set; }

    public virtual ICollection<ExamSection> ExamSections { get; set; } = new List<ExamSection>();

    public virtual ExamType? ExamType { get; set; }

    public virtual ICollection<Exam> InverseParentExam { get; set; } = new List<Exam>();

    public virtual Exam? ParentExam { get; set; }

    public virtual ICollection<UserExamResult> UserExamResults { get; set; } = new List<UserExamResult>();
}
