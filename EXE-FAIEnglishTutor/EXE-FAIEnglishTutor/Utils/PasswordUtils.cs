using EXE_FAIEnglishTutor.Models;
using Microsoft.AspNetCore.Identity;

namespace EXE_FAIEnglishTutor.Utils
{
    public static class PasswordUtils
    {
        private static readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public static string HashPassword(User user, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }

            return _passwordHasher.HashPassword(user, password);
        }

        public static bool VerifyPassword(User user, string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentException("Password and hashed password cannot be null or empty.");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
