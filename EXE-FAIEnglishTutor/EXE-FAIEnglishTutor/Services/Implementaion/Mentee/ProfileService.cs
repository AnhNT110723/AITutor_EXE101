using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using EXE_FAIEnglishTutor.Repositories.Interface.Mentee;
using Microsoft.Data.SqlClient;
using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Helpers;
using EXE_FAIEnglishTutor.Utils;
using EXE_FAIEnglishTutor.Repositories.Interface;
using PhoneNumbers;
using EXE_FAIEnglishTutor.Services.Interface;

namespace EXE_FAIEnglishTutor.Services.Implementaion.Mentee
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepo;
        private readonly IUserRepository _userRepo;
        private readonly IFileUploadService _fileUploadService;

        public ProfileService(IProfileRepository profileRepository, IUserRepository userRepo, IFileUploadService fileUploadService)
        {
            _profileRepo = profileRepository;
            _userRepo = userRepo;
            _fileUploadService = fileUploadService; 
        }
        public async Task<ProfileDto> GetUserById(int id)
        {
            try
            {
                User user = await _profileRepo.GetByIdAsync(id);

                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID {id} was not found.");
                }
                return new ProfileDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Dob = user.Dob,
                    Province = user.District ?? 0,
                    Phone = string.IsNullOrWhiteSpace(user.PhoneNumber) ? string.Empty : PhoneHelper.ConvertE164ToNational(user.PhoneNumber),
                    CountryCode = string.IsNullOrWhiteSpace(user.PhoneNumber) ? "vn" : PhoneHelper.GetRegionCodeFromE164(user.PhoneNumber).ToLower(),
                    Password = user.PasswordHash,
                    AvatarStr = user.Avatar,
                    Provider  = user.Provider,
                };
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Something went wrong with the database.");
            }
        }

        public async Task<User> SaveChangeAsync(ProfileDto profileDto)
        {
            try
            {
                User user = await _profileRepo.GetByIdAsync(profileDto.UserId);
                if (user == null)
                {
                    throw new KeyNotFoundException("NOTFOUND");
                }

                if(!string.IsNullOrEmpty(profileDto.Password))
                {
                    //Check old password
                    if (!string.IsNullOrEmpty(profileDto.OldPassword))
                    {
                        bool checkOldPass = PasswordUtils.VerifyPassword(user, profileDto.OldPassword, user.PasswordHash);
                        if (!checkOldPass)
                        {
                            throw new Exception("Old pass is wrong");
                        }
                    }

                    var newPassword = PasswordUtils.HashPassword(user, profileDto.Password);
                    user.PasswordHash = newPassword;
                }

                string phoneE164;
                try
                {
                    phoneE164 = PhoneHelper.ConvertToE164(profileDto.Phone, profileDto.CountryCode);
                }
                catch (NumberParseException ex)
                {
                    throw new Exception("Phone err");
                }

                profileDto.Phone = phoneE164;

                bool isPhoneExist = await _userRepo.IsPhoneNumberExists(profileDto.Phone);
                if (profileDto.Phone != user.PhoneNumber && isPhoneExist) {
                    throw new Exception("Phone Exist");
                }

                // Xử lý upload ảnh nếu có
                if (profileDto.Avatar != null)
                {
                    string avatarUrl = await _fileUploadService.UploadFileAsync(profileDto.Avatar);
                    user.Avatar = avatarUrl;
                } 
                if(!string.IsNullOrEmpty(profileDto.AvatarStr))
                {
                    user.Avatar = profileDto.AvatarStr;
                }

                // Cập nhật các trường khác
                user.FullName = profileDto.FullName ?? user.FullName; // Giữ giá trị cũ nếu null
                user.PhoneNumber = profileDto.Phone ?? user.PhoneNumber;
                user.Dob = profileDto.Dob ?? user.Dob; // Giữ giá trị cũ nếu null
                user.District = profileDto.Province > 0 ? profileDto.Province : user.District; // Chỉ cập nhật nếu hợp lệ

                await _profileRepo.SaveChangeAysnc();
                return user;
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Something went wrong with the database.");
            }

        }
    }
}
