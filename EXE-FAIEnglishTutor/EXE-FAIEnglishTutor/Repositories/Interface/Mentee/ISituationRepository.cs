﻿using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Repositories.Interface.Mentee
{
    public interface ISituationRepository
    {
        Task<List<Situation>> GetListSituationByRolePlay(int rolePlay);
        Task<Situation?> GetSituationByIdAsync(int situationId);

        Task<IEnumerable<Situation?>> GetAllSituation();

        Task<IEnumerable<Situation?>> GetTop8Situation();
        Task<List<Level?>> GetAllLevelAsync();
    }
}
