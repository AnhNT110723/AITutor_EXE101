using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EXE_FAIEnglishTutor.Services.Interface.Admin
{
    public interface ISituationAdminService
    {
        Task<List<Situation>> GetAllAsync();
        Task<SituationFilterViewModel> GetPagedAndFilteredAsync(string? search, int? levelId, int? typeId, int page, int pageSize);
        Task<Situation?> GetByIdAsync(int id);
        Task<bool> CreateAsync(Situation situation, IFormFile? imageFile);
        Task<bool> UpdateAsync(Situation situation, IFormFile? imageFile);
        Task<bool> DeleteAsync(int id);
    }
}
