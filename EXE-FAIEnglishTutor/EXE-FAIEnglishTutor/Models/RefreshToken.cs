using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class RefreshToken
{
    public int RefreshTokenId { get; set; }

    public string TokenCode { get; set; } = null!;

    public DateTime ExpiryDate { get; set; }

    public int UserId { get; set; }

    public string? DeviceInfo { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
