using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace EXE_FAIEnglishTutor.Services.Implementaion
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public User? AuthenticateUser(string email, string password)
        {
            var user = _userRepository.GetUserByEmail(email);
            if (user == null) return null;
            if (string.IsNullOrEmpty(user?.PasswordHash)) //check null pass word in db 
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded ? user : null;

        }

        public async Task<User?> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user;
        }
    }
}
