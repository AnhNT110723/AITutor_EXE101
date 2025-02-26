using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EXE_FAIEnglishTutor.Repositories.Implementation
{
    public class VerificationTokenRepositoryImpl : IVerificationTokenRepository
    {
        private readonly FaiEnglishContext _context;

        public VerificationTokenRepositoryImpl(FaiEnglishContext context)
        {
            _context = context;
        }

        public async Task<VerificationToken> findByToken(string token)
        {
            return await _context.VerificationTokens.Include(vt => vt.User).FirstOrDefaultAsync(x => x.TokenCode.Equals(token));
        }

        public async Task save(VerificationToken verificationToken)
        {
           _context.VerificationTokens.Add(verificationToken);
            await _context.SaveChangesAsync();
        }
    }
}
