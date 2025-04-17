using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Services.Interface;

namespace EXE_FAIEnglishTutor.Services.Implementaion
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;

        public RefreshTokenService(IRefreshTokenRepository repository)
        {
            _repository = repository;
        }

        public async Task SaveRefreshTokenAsync(RefreshToken token)
        {
            await _repository.AddAsync(token);
        }
    }
}
