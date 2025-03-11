using System.ComponentModel.DataAnnotations;

namespace EXE_FAIEnglishTutor.Dtos
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Oop...! Token is required")]
        public string Token { get; set; } = "";

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
    }
}
