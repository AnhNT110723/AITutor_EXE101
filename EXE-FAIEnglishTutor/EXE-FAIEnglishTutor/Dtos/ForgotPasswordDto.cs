using System.ComponentModel.DataAnnotations;

namespace EXE_FAIEnglishTutor.Dtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Nhập Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]

        public string Email { get; set; } = "";
    }
}
