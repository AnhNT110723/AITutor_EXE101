using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IRegisterUserService
    {
        Task registerUser(RegisterDto model);
        Task<User> FindOrCreateExternalUserAsync(string email, string fullName, string provider, string providerId);
        Task<string> confirmToken(string token);
        Task resendActivation(string email);
    }
}
