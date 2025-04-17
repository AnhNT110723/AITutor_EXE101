using EXE_FAIEnglishTutor.Common;
using EXE_FAIEnglishTutor.Enums;
using System.ComponentModel.DataAnnotations;

namespace EXE_FAIEnglishTutor.Dtos
{
    public class ProfileDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "Please select a country")]
        public string CountryCode { get; set; } = "VN";

        [Required(ErrorMessage = "Phone is required")]
        [PhoneNumber(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = "";


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
        ErrorMessage = "Password must have (A-Z), (a-z), (0-9), a special char, and be 8+ chars.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Re-entering password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string RePassword { get; set; }

        public string? AvatarStr { get; set; }
        public IFormFile? Avatar { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}
