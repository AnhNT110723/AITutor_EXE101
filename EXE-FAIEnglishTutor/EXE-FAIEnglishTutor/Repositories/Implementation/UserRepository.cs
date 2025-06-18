using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace EXE_FAIEnglishTutor.Repositories.Implementation
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {

        public UserRepository(FaiEnglishContext context) : base(context)
        {
        }

        public async Task<User> FindExternalUserByProviderAsync(string provider, string providerId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Provider == provider && u.ProviderId == providerId);
        }

        public async Task<IEnumerable<User>> GetAllUserAsync()
        {
            return await _context.Users
                .Include(u => u.Roles).ToListAsync();
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _context.Users.CountAsync();
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users.Include(x => x.Roles).FirstOrDefault(u => u.Email.Equals(email));
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Roles).FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<bool> IsPhoneNumberExists(string phoneNumber)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber.Equals(phoneNumber));
            return user != null;
        }

        public async Task save(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                Console.WriteLine("Err: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
            }

        }

        public async Task SaveChangeAsync(User user)
        {
           await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
