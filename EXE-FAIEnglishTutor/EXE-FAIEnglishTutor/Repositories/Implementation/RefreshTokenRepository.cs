using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;

namespace EXE_FAIEnglishTutor.Repositories.Implementation
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly FaiEnglishContext _context;

        public RefreshTokenRepository(FaiEnglishContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }
    }
}
