namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IForgotPasswordService
    {
        Task ForGotPassword(string email);

        Task<string> CheckTokenValid(string token);

        Task ResetPassword(string email, string password, string confirmPassword);
    }
}
