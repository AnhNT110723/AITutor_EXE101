using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class UserExamPartSelection
{
    public int SelectionId { get; set; }

    public int? ResultId { get; set; }

    public int? PartId { get; set; }

    public virtual ExamPart? Part { get; set; }

    public virtual UserExamResult? Result { get; set; }
}
