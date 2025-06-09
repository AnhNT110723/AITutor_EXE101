using EXE_FAIEnglishTutor.Repositories.Interface.Mentee;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace EXE_FAIEnglishTutor.Repositories.Implementation.Mentee
{
    public class SituationRepository : BaseRepository<Situation>, ISituationRepository
    {
        public SituationRepository(FaiEnglishContext context) : base(context)
        {
        }

        public async Task<List<Level?>> GetAllLevelAsync()
        {
            var x = await _context.Levels.ToListAsync();
            return x;
        }

        public async Task<List<Situation>> GetListSituationByRolePlay(int rolePlay)
        {
            var x = await _context.Situations.Include(x => x.Level).Where(t => t.TypeId == rolePlay).ToListAsync();
            return x;
        }

        public async Task<Situation?> GetSituationByIdAsync(int situationId)
        {
            var x = await _context.Situations.Include(x => x.Level).Where(t => t.SituatuonId == situationId).FirstOrDefaultAsync();
            return x;
        }
    }
}
