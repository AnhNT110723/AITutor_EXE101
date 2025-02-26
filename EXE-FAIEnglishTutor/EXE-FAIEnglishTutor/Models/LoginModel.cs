using System.ComponentModel.DataAnnotations;

namespace EXE_FAIEnglishTutor.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email hoặc số điện thoại là bắt buộc")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu bắt buộc")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
