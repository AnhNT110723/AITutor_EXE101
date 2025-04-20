using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class Question
{
    public int QuestionId { get; set; }

    public int? SectionId { get; set; }

    public string? QuestionText { get; set; }

    public string? AudioUrl { get; set; }

    public string? ImageUrl { get; set; }

    public string? QuestionType { get; set; }

    public string? Explanation { get; set; }

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual ExamSection? Section { get; set; }

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
