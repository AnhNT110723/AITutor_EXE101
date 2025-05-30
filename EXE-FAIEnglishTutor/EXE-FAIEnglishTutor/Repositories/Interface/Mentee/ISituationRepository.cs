using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Repositories.Interface.Mentee
{
    public interface ISituationRepository
    {
        Task<List<Situation>> GetListSituationByRolePlay(int rolePlay);
        Task<Situation?> GetSituationByIdAsync(int situationId);
    }
}
