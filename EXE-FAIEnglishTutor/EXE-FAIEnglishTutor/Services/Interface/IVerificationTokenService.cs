using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IVerificationTokenService
    {
        void createVertificationToken(User user, string token);
        void updateVerificationToken(User user, string newToken);

        void updateResetPasswordToken(User user, String newToken);
        Task<VerificationToken> getVerificationToken(string token);
    }
}
