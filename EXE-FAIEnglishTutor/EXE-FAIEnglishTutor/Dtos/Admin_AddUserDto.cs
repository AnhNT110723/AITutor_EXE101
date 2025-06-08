using EXE_FAIEnglishTutor.Attribute;
using EXE_FAIEnglishTutor.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EXE_FAIEnglishTutor.Dtos
{
    public class Admin_AddUserDto
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "Please select a country")]
        public string CountryCode { get; set; } = "VN";

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Phone number must follow Vietnamese format (e.g., 0912345678).")]
        public string? PhoneNumber { get; set; }

        [NotMapped]
        [FileValidation(ErrorMessage = "Image file must be .jpg, .jpeg, or .png and not exceed 5MB.")]
        public IFormFile? Avatar { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [PastDate(ErrorMessage = "Date of birth must be in the past.")]
        public DateTime? Dob { get; set; }


        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*(){}[\]_\-+=~`|:;""'<>,.?])(?=.*[a-z])(?=.*[A-Z]).{8,}$",
           ErrorMessage = "Password must contain uppercase (A-Z), lowercase (a-z), number (0-9), and a special character.")]
        [DataType(DataType.Password)]
        public string? Password{ get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Confirm password does not match.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[!@#$%^&*(){}[\]_\-+=~`|:;""'<>,.?])(?=.*[a-z])(?=.*[A-Z]).{8,}$",
           ErrorMessage = "Confirm password must contain uppercase (A-Z), lowercase (a-z), number (0-9), and a special character.")]
        [DataType(DataType.Password)]
        public string? RePassword { get; set; }

        public int? RoleId { get; set; }

    }
}
