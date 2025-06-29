﻿using EXE_FAIEnglishTutor.Models;

namespace EXE_FAIEnglishTutor.Services.Interface.Mentee
{
    public interface ISituationService
    {
        Task<List<Situation>> GetListSituationByRolePlay(int rolePlay, string keyword = "", string category = "");
        Task<Situation?> GetSituationByIdAsync(int situationId);

        Task<IEnumerable<Situation?>> GetAllSituation();

        Task<IEnumerable<Situation?>> GetTop8Situation();
        Task<List<Level?>> GetAllLevelAsync();
    }
}
