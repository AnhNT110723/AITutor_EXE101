using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;

namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IUserService
    {
        User? AuthenticateUser(string email, string password);

        Task<User?> GetUserById(int id);
    }
}
