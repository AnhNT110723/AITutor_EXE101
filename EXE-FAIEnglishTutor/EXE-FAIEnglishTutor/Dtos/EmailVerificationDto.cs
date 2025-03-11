namespace EXE_FAIEnglishTutor.Dtos
{
    public class EmailVerificationDto
    {
        public string FullName { get; set; } = "";
        public string VerificationLink { get; set; } = ""; 
        public DateTime ExpirationTime { get; set; }

        public string Subject { get; set; } = "";

        public bool isVertification { get; set; } = false;
        public bool isResendPassword { get; set; } = false;

    }
}
