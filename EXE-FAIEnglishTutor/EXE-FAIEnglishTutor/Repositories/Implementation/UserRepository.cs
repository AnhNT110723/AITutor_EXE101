using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;

namespace EXE_FAIEnglishTutor.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly FaiEnglishContext _context;

        public UserRepository(FaiEnglishContext context)
        {
            _context = context;
        }
        public User? GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email.Equals(email));
        }

        public bool IsPhoneNumberExists(string phoneNumber)
        {
            var user = _context.Users.FirstOrDefault(u => u.PhoneNumber.Equals(phoneNumber));
            return user != null;
        }

        public async Task save(User user)
        {
            _context.Users.Add(user);
           await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
