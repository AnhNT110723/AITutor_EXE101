using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FullName { get; set; }

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Avatar { get; set; }

    public DateTime? Dob { get; set; }

    public int? District { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? ExpiryDate { get; set; }

    public string Provider { get; set; } = null!;

    public string? ProviderId { get; set; }

    public DateTime? LastLogin { get; set; }

    public virtual ICollection<Feeback> Feebacks { get; set; } = new List<Feeback>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual VerificationToken? VerificationToken { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
