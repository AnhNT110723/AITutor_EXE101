using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories.Interface.Mentee;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;

namespace EXE_FAIEnglishTutor.Services.Implementaion.Mentee
{
    public class SituationService : ISituationService
    {
        private readonly ISituationRepository _situationRepository;
        public SituationService(ISituationRepository situationRepository) 
        {
            _situationRepository = situationRepository;
        }

        public async Task<IEnumerable<Situation?>> GetAllSituation()
        {
            return await _situationRepository.GetAllSituation();
        }

        public async Task<List<Situation>> GetListSituationByRolePlay(int rolePlay)
        {
            var listSituations = await _situationRepository.GetListSituationByRolePlay(rolePlay);
            return listSituations;
        }

        public async Task<Situation?> GetSituationByIdAsync(int situationId)
        {
            var situation = await _situationRepository.GetSituationByIdAsync(situationId);
            return situation;
        }

        public async Task<IEnumerable<Situation?>> GetTop8Situation()
        {
            return await _situationRepository.GetTop8Situation();
        }
    }
}
