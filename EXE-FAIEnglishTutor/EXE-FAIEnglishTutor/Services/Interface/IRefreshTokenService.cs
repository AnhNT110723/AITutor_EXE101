using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IRefreshTokenService
    {
        Task SaveRefreshTokenAsync(RefreshToken token);
    }
}
