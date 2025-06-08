using EXE_FAIEnglishTutor.Enums;
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

        public async Task AddUserAsync(User user)
        {
           await _userRepository.AddAsync(user);
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

        public async Task BlockUserAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            user.Status = EXE_FAIEnglishTutor.Enums.AccountStatus.LOCKED;
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            await _userRepository.DeleteAsync(user);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUserAsync();
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await _userRepository.GetTotalUsersAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return user;
        }

        public async Task UpdateUser(User user)
        {
             await _userRepository.Update(user);
        }
    }
}
