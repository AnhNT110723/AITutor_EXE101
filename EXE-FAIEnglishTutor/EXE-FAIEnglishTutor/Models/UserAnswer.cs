using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class UserAnswer
{
    public int UserAnswerId { get; set; }

    public int? ResultId { get; set; }

    public int? QuestionId { get; set; }

    public int? SelectedAnswerId { get; set; }

    public bool? IsCorrect { get; set; }

    public virtual Question? Question { get; set; }

    public virtual UserExamResult? Result { get; set; }

    public virtual Answer? SelectedAnswer { get; set; }
}
