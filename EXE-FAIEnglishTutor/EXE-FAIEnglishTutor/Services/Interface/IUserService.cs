using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IUserService
    {
        User? AuthenticateUser(string email, string password);


    }
}
