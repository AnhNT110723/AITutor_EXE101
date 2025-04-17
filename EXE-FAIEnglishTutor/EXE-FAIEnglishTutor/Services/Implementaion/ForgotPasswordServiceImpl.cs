using EXE_FAIEnglishTutor.Common;
using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Enums;
using EXE_FAIEnglishTutor.Mail;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace EXE_FAIEnglishTutor.Services.Implementaion
{
    public class ForgotPasswordServiceImpl : IForgotPasswordService
    {

        private readonly IUserRepository _userRepository;
        private readonly IVerificationTokenService _verificationTokenService;
        private readonly EmailSendVetification _emailSendVetification;

        public ForgotPasswordServiceImpl(IUserRepository userRepository, IVerificationTokenService verificationTokenService, EmailSendVetification emailSendVetification)
        {
            _userRepository = userRepository;
            _verificationTokenService = verificationTokenService;
            _emailSendVetification = emailSendVetification;
        }
        public async Task<string> CheckTokenValid(string token)
        {
            VerificationToken verificationToken = await _verificationTokenService.getVerificationToken(token);
            if (verificationToken == null) {
                return "Invalid token";
            }

            if (verificationToken.ExpiryDate < DateTime.Now) {
                return "Token expired";
            }

            return "Token Valid";  
        }

        public async Task ForGotPassword(string email)
        {
            User user =  await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("NotFoundAccount");
            }

            if (user.Status.Equals(AccountStatus.LOCKED))
            {
                throw new Exception("Account Locked");
            }
            if (user.Status.Equals(AccountStatus.PENDING))
            {
                throw new Exception("Account not activate");
            }

            //Create token for reset password
            string token = Guid.NewGuid().ToString();
            await _verificationTokenService.updateVerificationToken(user, token, Constants.EXPIRATION_FORGOTPASSWORD); // update token in vertification by userid

            //Send mail to client a email witk link resetpassword

            var emailModel = new EmailVerificationDto
            {
                FullName = user.FullName,
                VerificationLink = "http://localhost:5037/Account/ResetPassword?token=" + token + "&Email=" + user.Email,
                ExpirationTime = DateTime.UtcNow.AddMinutes(Constants.EXPIRATION_FORGOTPASSWORD),
                isResendPassword = true,
                isVertification = false,
                Subject = "Đổi mật khẩu của bạn - FAI English",

            };
            await _emailSendVetification.SendVerificationEmailAsync(user.Email, emailModel);
        }

        public async Task ResetPassword(string email, string password, string confirmPassword)
        {
            var passwordHasher = new PasswordHasher<User>();
            User user = await _userRepository.GetUserByEmailAsync(email);
            user.PasswordHash = passwordHasher.HashPassword(user, password);
            await _userRepository.Update(user);
        }
    }
}
