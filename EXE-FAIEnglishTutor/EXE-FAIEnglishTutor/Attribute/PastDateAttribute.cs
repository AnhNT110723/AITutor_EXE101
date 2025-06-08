using System.ComponentModel.DataAnnotations;

namespace EXE_FAIEnglishTutor.Attribute
{
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateOnly date)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                if (date >= today)
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }
}
