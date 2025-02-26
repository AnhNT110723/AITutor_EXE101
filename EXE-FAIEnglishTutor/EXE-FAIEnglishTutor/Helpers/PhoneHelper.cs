using PhoneNumbers;

namespace EXE_FAIEnglishTutor.Helpers
{
    public static class PhoneHelper
    {
        public static string ConvertToE164(string phoneNumber, string region)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();
            var numberProto = phoneUtil.Parse(phoneNumber, region);
            if (!phoneUtil.IsValidNumber(numberProto))
            {
                throw new PhoneNumbers.NumberParseException(PhoneNumbers.ErrorType.NOT_A_NUMBER, "Invalid phone number.");
            }
            return phoneUtil.Format(numberProto, PhoneNumberFormat.E164);
        }
    }
}
