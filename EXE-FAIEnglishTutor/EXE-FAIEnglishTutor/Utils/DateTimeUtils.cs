namespace EXE_FAIEnglishTutor.Utils
{
    public class DateTimeUtils
    {
        public static DateTime calculateExpiryDate(int expiryTimeInMinutes)
        {
            return DateTime.Now.AddMinutes(expiryTimeInMinutes);
        }
    }


}
