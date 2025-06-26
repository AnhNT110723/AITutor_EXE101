namespace EXE_FAIEnglishTutor.Common
{
    public static class Constants
    {
        public const int EXPIRATION = 60 * 24;// kich hoat tai khoan (1day)
        public const int EXPIRATION_FORGOTPASSWORD = 15;// kich hoat tai khoan (15phut)

        public const int ROLE_PLAY = 1;
        public const int LISTENING = 2;
        public const int PRONOUNCE = 3;

        // tiếng mà muốn dịch sang
        public const string TARGET_LANG = "vi";

        //Base Url
        public const string BASE_URL = "https://faienglish.xyz/";

        //Thông tin thẻ ngân hàng
        public const string BANK_ID = "MB";
        public const string ACCOUNT_NO = "0968640321111";
        public const string ACCOUNT_NAME = "NGUYEN TUAN ANH";
        public const string CONTENT = "Nâng cấp tài khoản FAI";


        //Status payment
        public const string COMPLETE = "Completed";
        public const string FAIL = "Failed";
        public const string PENDING = "Pending";
        public const string CANCEL = "Cancel";

        // gói nâng cấp
        public const decimal MEMBER = 39000;
        public const decimal VIP = 200000;
        public const decimal ADVANCE = 640000;








    }
}
