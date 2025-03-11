using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Repositories.Interface
{
    public interface IVerificationTokenRepository
    { 
        Task save(VerificationToken verificationToken);

        Task<VerificationToken> findByToken(string token);
        Task<VerificationToken> findByUserId(int ID);

        Task UpdateToken(VerificationToken token);
    }
}
