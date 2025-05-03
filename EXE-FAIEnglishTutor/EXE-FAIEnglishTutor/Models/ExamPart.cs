using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class ExamPart
{
    public int PartId { get; set; }

    public int? ExamTypeId { get; set; }

    public string? PartType { get; set; }

    public string? PartName { get; set; }

    public string? Description { get; set; }

    public int? DefaultDuration { get; set; }

    public virtual ICollection<ExamSection> ExamSections { get; set; } = new List<ExamSection>();

    public virtual ExamType? ExamType { get; set; }

    public virtual ICollection<UserExamPartSelection> UserExamPartSelections { get; set; } = new List<UserExamPartSelection>();
}
