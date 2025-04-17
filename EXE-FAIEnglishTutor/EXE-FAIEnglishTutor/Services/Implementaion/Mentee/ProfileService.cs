using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using EXE_FAIEnglishTutor.Repositories.Interface.Mentee;
using Microsoft.Data.SqlClient;
using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Helpers;

namespace EXE_FAIEnglishTutor.Services.Implementaion.Mentee
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepo;

        public ProfileService(IProfileRepository profileRepository)
        {
            _profileRepo = profileRepository;
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
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = string.IsNullOrWhiteSpace(user.PhoneNumber) ? string.Empty: PhoneHelper.ConvertE164ToNational(user.PhoneNumber),
                    CountryCode = string.IsNullOrWhiteSpace(user.PhoneNumber) ? "vn"  : PhoneHelper.GetRegionCodeFromE164(user.PhoneNumber).ToLower(),
                    Password = user.PasswordHash,
                    AvatarStr = user.Avatar,
                    ExpiryDate = user.ExpiryDate,
                };
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Something went wrong with the database.");
            }
        }
    }
}
