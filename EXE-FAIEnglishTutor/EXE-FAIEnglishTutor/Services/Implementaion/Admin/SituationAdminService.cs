using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface;
using EXE_FAIEnglishTutor.Services.Interface.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EXE_FAIEnglishTutor.Services.Implementaion.Admin
{
    public class SituationAdminService : ISituationAdminService
    {
        private readonly FaiEnglishContext _context;
        private readonly IFileUploadService _fileUploadService;

        public SituationAdminService(FaiEnglishContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }

        public async Task<List<Situation>> GetAllAsync()
        {
            return await _context.Situations
                .Include(s => s.Level)
                .Include(s => s.Type)
                .ToListAsync();
        }

        public async Task<SituationFilterViewModel> GetPagedAndFilteredAsync(string? search, int? levelId, int? typeId, int page, int pageSize)
        {
            var query = _context.Situations
                .Include(s => s.Level)
                .Include(s => s.Type)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim();
                query = query.Where(x => x.SituationName.Contains(s) || (x.Description != null && x.Description.Contains(s)));
            }

            if (levelId.HasValue && levelId.Value > 0)
            {
                query = query.Where(x => x.LevelId == levelId.Value);
            }

            if (typeId.HasValue && typeId.Value > 0)
            {
                query = query.Where(x => x.TypeId == typeId.Value);
            }

            var totalItems = await query.CountAsync();
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var items = await query
                .OrderByDescending(x => x.SituatuonId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new SituationFilterViewModel
            {
                Items = items,
                Search = search,
                LevelId = levelId,
                TypeId = typeId,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task<Situation?> GetByIdAsync(int id)
        {
            return await _context.Situations
                .Include(s => s.Level)
                .Include(s => s.Type)
                .FirstOrDefaultAsync(s => s.SituatuonId == id);
        }

        public async Task<bool> CreateAsync(Situation situation, IFormFile? imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var imageUrl = await _fileUploadService.UploadFileAsync(imageFile, "situations");
                situation.ImageUrl = imageUrl;
            }

            situation.CreatedAt = DateTime.UtcNow;
            
            _context.Situations.Add(situation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Situation situation, IFormFile? imageFile)
        {
            try
            {
                var existingSituation = await _context.Situations.FindAsync(situation.SituatuonId);
                if (existingSituation == null)
                    return false;

                // Update fields
                existingSituation.SituationName = situation.SituationName;
                existingSituation.Description = situation.Description;
                existingSituation.RoleAi = situation.RoleAi;
                existingSituation.RoleUser = situation.RoleUser;
                existingSituation.LearningObjectives = situation.LearningObjectives;
                existingSituation.TypeId = situation.TypeId;
                existingSituation.LevelId = situation.LevelId;

                // Xử lý upload ảnh nếu có
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageUrl = await _fileUploadService.UploadFileAsync(imageFile, "situations");
                    existingSituation.ImageUrl = imageUrl;
                }

                // Cập nhật lại RowVersion (rất quan trọng cho Optimistic Concurrency)
                _context.Entry(existingSituation).Property(e => e.RowVersion).OriginalValue = situation.RowVersion;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Re-throw để Controller bắt và báo lỗi cho Admin
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var situation = await _context.Situations.FindAsync(id);
            if (situation != null)
            {
                _context.Situations.Remove(situation);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
