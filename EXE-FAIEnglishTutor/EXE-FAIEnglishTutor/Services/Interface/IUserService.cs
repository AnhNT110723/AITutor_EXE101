using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;

namespace EXE_FAIEnglishTutor.Services.Interface
{
    public interface IUserService
    {
        User? AuthenticateUser(string email, string password);

        Task<User?> GetUserById(int id);

        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);

        Task UpdateUser(User user);

        Task AddUserAsync(User user);
        
        Task SaveChangeAsync(User user);

        Task DeleteUserAsync(int id);

        Task BlockUserAsync(int id);

        Task<int> GetTotalUsersAsync();


    }
}
