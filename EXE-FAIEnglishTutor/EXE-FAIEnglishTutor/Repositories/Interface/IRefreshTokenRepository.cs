using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Repositories.Interface
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
    }
}
