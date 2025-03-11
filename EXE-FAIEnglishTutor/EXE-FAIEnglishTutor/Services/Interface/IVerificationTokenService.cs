using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IVerificationTokenService
    {
        void createVertificationToken(User user, string token);
        Task updateVerificationToken(User user, string newToken, int expireTime);
        Task<VerificationToken> getVerificationToken(string token);
    }
}
