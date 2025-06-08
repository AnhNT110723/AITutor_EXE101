using System.ComponentModel.DataAnnotations;

namespace EXE_FAIEnglishTutor.Attribute
{
    public class FileValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var validExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExt = Path.GetExtension(file.FileName).ToLower();
                if (!validExtensions.Contains(fileExt))
                {
                    return new ValidationResult("Only .jpg, .jpeg, or .png files are allowed.");
                }
                if (file.Length > 5 * 1024 * 1024) // 5MB
                {
                    return new ValidationResult("File size must not exceed 5MB.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
