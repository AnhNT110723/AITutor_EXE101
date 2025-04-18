using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Repositories.Interface.Mentee;

namespace EXE_FAIEnglishTutor.Repositories.Implementation.Mentee
{
    public class ProfileRepository :  BaseRepository<User>,IProfileRepository
    {
        public ProfileRepository(FaiEnglishContext context) : base(context) { }

        public async Task SaveChangeAysnc()
        {
            await _context.SaveChangesAsync();
        }
    }
}
