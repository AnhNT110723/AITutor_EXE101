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

        public async Task<List<Level?>> GetAllLevelAsync()
        {
            var levels = await _situationRepository.GetAllLevelAsync();
            return levels;
        }

        public async Task<List<Situation>> GetListSituationByRolePlay(int rolePlay, string keyword = "", string category = "")
        {

            var listSituations = await _situationRepository.GetListSituationByRolePlay(rolePlay);
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                listSituations = listSituations
                        .Where(s => s.SituationName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        .ToList();
            }
            if (!string.IsNullOrWhiteSpace(category))
            {
                if (int.TryParse(category, out int categoryId))
                {
                    listSituations = listSituations
                        .Where(s => s.Level.LevelId == categoryId)
                        .ToList();
                }
                else
                {
                    // Optional: nếu không parse được thì bỏ lọc hoặc trả về rỗng
                    listSituations = new List<Situation>(); // hoặc giữ nguyên không lọc
                }
            }
            return listSituations;
        }

        public async Task<Situation?> GetSituationByIdAsync(int situationId)
        {
            var situation = await _situationRepository.GetSituationByIdAsync(situationId);
            return situation;
        }
    }
}
