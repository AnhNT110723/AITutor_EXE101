using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Helpers;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface;
using EXE_FAIEnglishTutor.Enums;
using Microsoft.AspNetCore.Identity;
using PhoneNumbers;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Mail;

namespace EXE_FAIEnglishTutor.Services.Implimentaion
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
            if (_userRepository.GetUserByEmail(model.Email) != null)
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

            if (_userRepository.IsPhoneNumberExists(model.Phone))
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
            await _emailSendVetification.SendVerificationEmailAsync(model.Email, token);

        }


        public async Task<string> confirmToken(string token)
        {
            VerificationToken verificationToken = await _verificationTokenService.getVerificationToken(token);
            if (verificationToken == null)
            {
                return "Invalid Token";
            }

            if(verificationToken.ExpiryDate < DateTime.Now)
            {
                return "Token expired";
            }

            User? user = verificationToken.User;
            if (user == null) {
                return "UserNotFound";
            }
            if(user.Status.Equals(AccountStatus.LOCKED))
            {
                return "Account Locked";
            }
            if(user.Status.Equals(AccountStatus.ACTIVATED))
            {
                return "Account activated";
            }

            // Activate user
            user.Status = AccountStatus.ACTIVATED;
            await _userRepository.Update(user);
            return "Token valid";
        }
    }
}
