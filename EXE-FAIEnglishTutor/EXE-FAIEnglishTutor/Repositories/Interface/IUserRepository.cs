using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Repositories.Interface
{
    public interface IUserRepository
    {
        User? GetUserByEmail(string email);

        bool IsPhoneNumberExists(string phoneNumber);

        Task save(User user);
        
        Task Update(User user);
    }
}
