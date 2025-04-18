using System;
using System.ComponentModel.DataAnnotations;

namespace EXE_FAIEnglishTutor.Common
{
    public class CustomDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Ngày sinh là bắt buộc");
            }

            DateTime date = (DateTime)value;
            DateTime today = DateTime.Today;

            // Tính tuổi
            int age = today.Year - date.Year;
            if (date.Date > today.AddYears(-age))
            {
                age--;
            }

            // Kiểm tra ngày trong tương lai
            if (date > today)
            {
                return new ValidationResult("Ngày sinh không thể trong tương lai");
            }

            // Kiểm tra tuổi tối thiểu
            if (age < 5)
            {
                return new ValidationResult("Bạn phải đủ 5 tuổi");
            }

            // Kiểm tra tuổi tối đa
            if (age > 100)
            {
                return new ValidationResult("Ngày sinh không hợp lệ");
            }

            return ValidationResult.Success;
        }
    }
} 