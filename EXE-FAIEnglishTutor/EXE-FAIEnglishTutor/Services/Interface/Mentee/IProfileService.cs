using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface.Mentee
{
    public interface IProfileService
    {
        Task<ProfileDto> GetUserById(int id);
        Task SaveChangeAsync(ProfileDto profileDto);
    }
}
