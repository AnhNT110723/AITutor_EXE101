using EXE_FAIEnglishTutor.Dtos;

namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IRegisterUserService
    {
        Task registerUser(RegisterDto model);

        Task<string> confirmToken(string token);
    }
}
