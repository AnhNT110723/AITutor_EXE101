using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Repositories.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User? GetUserByEmail(string email);
        Task<User> GetUserByEmailAsync(string email);

        Task<User> FindExternalUserByProviderAsync(string provider, string providerId);

        Task<bool> IsPhoneNumberExists(string phoneNumber);

        Task save(User user);
        
        Task Update(User user);
        
        Task SaveChangeAsync(User user);
        Task<IEnumerable<User>> GetAllUserAsync();

        Task<int> GetTotalUsersAsync();

        Task<User> GetUserByIdAsync(int id);



    }
}
