using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using PhoneNumbers;
using System.ComponentModel.DataAnnotations;

namespace EXE_FAIEnglishTutor.Common
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public string DefaultRegion { get; set; } = "VN"; // Mặc định là Việt Nam
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            string phoneNumber = value as string;
            if (string.IsNullOrEmpty(phoneNumber))
                return ValidationResult.Success;

            // Lấy giá trị CountryCode từ model nếu có
            var model = validationContext.ObjectInstance as RegisterDto;
            string region = model?.CountryCode ?? DefaultRegion;

            try
            {
                var phoneUtil = PhoneNumberUtil.GetInstance();
                var numberProto = phoneUtil.Parse(phoneNumber, region);
                if (phoneUtil.IsValidNumber(numberProto))
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage ?? "Số điện thoại không hợp lệ cho quốc gia này.");
                }
            }
            catch (NumberParseException ex)
            {
                return new ValidationResult(ErrorMessage ?? $"Lỗi định dạng: {ex.Message}");
            }
        }
    }
}
