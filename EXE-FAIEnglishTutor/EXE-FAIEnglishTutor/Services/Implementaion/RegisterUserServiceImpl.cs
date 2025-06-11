using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Helpers;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface;
using EXE_FAIEnglishTutor.Enums;
using Microsoft.AspNetCore.Identity;
using PhoneNumbers;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Mail;
using EXE_FAIEnglishTutor.Common;
using Microsoft.EntityFrameworkCore;

namespace EXE_FAIEnglishTutor.Services.Implementaion
{
    public class RegisterUserServiceImpl : IRegisterUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IVerificationTokenService _verificationTokenService;
        private readonly EmailSendVetification _emailSendVetification;

        public RegisterUserServiceImpl(IUserRepository userRepository, IRoleRepository roleRepository, IVerificationTokenService verificationTokenService,
            EmailSendVetification emailSendVetification)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _verificationTokenService = verificationTokenService;
            _emailSendVetification = emailSendVetification;
        }

        public async Task registerUser(RegisterDto model)
        {
            if ( await _userRepository.GetUserByEmailAsync(model.Email) != null)
            {
                //Check email is exists
                throw new Exception("Email exist");
            }

            string phoneE164;
            try
            {
                phoneE164 = PhoneHelper.ConvertToE164(model.Phone, model.CountryCode);
            }
            catch (NumberParseException ex)
            {
                throw new Exception("Phone err");
            }

            model.Phone = phoneE164;

            if ( await _userRepository.IsPhoneNumberExists(model.Phone))
            {
                //Check phone is exists
                throw new Exception("Phone exist");
            }

            //SAVE TO DATABASE

            User user = new User
            {
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.Phone,
                CreatedAt = DateTime.UtcNow,
                Status = AccountStatus.PENDING,
                Avatar = "/Images/user_icon.webp",
                Provider = "Local",
            };

            // Mã hóa mật khẩu và lưu vào thuộc tính PasswordHash
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
            Role defaultRole = _roleRepository.GetRoleByName("mentee");
            if (defaultRole != null)
            {
                // Thêm role vào navigation property (EF Core sẽ tự động lưu quan hệ vào bảng trung gian)
                user.Roles.Add(defaultRole);
            }

            await _userRepository.save(user);
            // Create verification token
            string token = Guid.NewGuid().ToString();
            _verificationTokenService.createVertificationToken(user, token);

            // Send confirmation email
            var emailModel = new EmailVerificationDto
            {
                FullName = user.FullName,
                VerificationLink = Constants.BASE_URL + "Account/ConfirmVerificationToken?token=" + token,
                ExpirationTime = DateTime.UtcNow.AddMinutes(Constants.EXPIRATION),
                isResendPassword = false,
                isVertification = true,
                Subject = "Xác thực email của bạn - FAI English",

            };
            await _emailSendVetification.SendVerificationEmailAsync(model.Email, emailModel);

        }


        public async Task<string> confirmToken(string token)
        {
            VerificationToken verificationToken = await _verificationTokenService.getVerificationToken(token);
            if (verificationToken == null)
            {
                return "Invalid Token";
            }

            if (verificationToken.ExpiryDate < DateTime.Now)
            {
                return "Token expired";
            }

            User? user = verificationToken.User;
            if (user == null)
            {
                return "UserNotFound";
            }
            if (user.Status.Equals(AccountStatus.LOCKED))
            {
                return "Account Locked";
            }
            if (user.Status.Equals(AccountStatus.ACTIVATED))
            {
                return "Account activated";
            }

            // Activate user
            user.Status = AccountStatus.ACTIVATED;
            await _userRepository.Update(user);
            return "Token valid";
        }

        public async Task resendActivation(string email)
        {
            User? user = _userRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("NotFound");
            }
            if (user.Status.Equals(AccountStatus.LOCKED))
            {
                throw new Exception("Account Locked");
            }
            if (user.Status.Equals(AccountStatus.ACTIVATED))
            {
                throw new Exception("Account activated");
            }


            string newToken = Guid.NewGuid().ToString();
            await _verificationTokenService.updateVerificationToken(user, newToken, Constants.EXPIRATION);

            // Gửi email bất đồng bộ
            _ = Task.Run(async () =>
            {
                try
                {
                    var emailModel = new EmailVerificationDto
                    {
                        FullName = user.FullName,
                        VerificationLink = Constants.BASE_URL + "/Account/ConfirmVerificationToken?token=" + newToken,
                        ExpirationTime = DateTime.UtcNow.AddMinutes(Constants.EXPIRATION),
                        isResendPassword = false,
                        isVertification = true,
                        Subject = "Xác thực email của bạn - FAI English",

                    };
                    await _emailSendVetification.SendVerificationEmailAsync(user.Email, emailModel);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi gửi email: {ex.Message}");
                }
            });

        }

        public async Task<User> FindOrCreateExternalUserAsync(string email, string fullName, string provider, string providerId)
        {
            var user = await _userRepository.FindExternalUserByProviderAsync(provider, providerId);
            if (user == null)
            {

                // Kiểm tra email trùng với user khác
                var existingUser = await _userRepository.GetUserByEmailAsync(email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email already exists with a different provider.");
                }

                user = new User
                {
                    Email = email,
                    FullName = fullName,
                    Provider = provider,
                    ProviderId = providerId,
                    PasswordHash = null,
                    Avatar = "/Images/user_icon.webp",
                    CreatedAt = DateTime.UtcNow,
                    Status = AccountStatus.ACTIVATED,
                    LastLogin = DateTime.UtcNow
                };
                Role defaultRole = _roleRepository.GetRoleByName("mentee");
                if (defaultRole != null)
                {
                    // Thêm role vào navigation property (EF Core sẽ tự động lưu quan hệ vào bảng trung gian)
                    user.Roles.Add(defaultRole);
                }

                await _userRepository.save(user);
            }
            else
            {
                user.LastLogin = DateTime.UtcNow;
                await _userRepository.Update(user);
            }


            return user;
        }
    }
}
