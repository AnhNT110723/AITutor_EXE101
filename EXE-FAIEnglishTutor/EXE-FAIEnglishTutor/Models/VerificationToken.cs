using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class VerificationToken
{
    public int TokenId { get; set; }

    public string? TokenCode { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
