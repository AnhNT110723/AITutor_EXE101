using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class UserExamResult
{
    public int ResultId { get; set; }

    public int? UserId { get; set; }

    public int? ExamId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public double? Score { get; set; }

    public virtual Exam? Exam { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
