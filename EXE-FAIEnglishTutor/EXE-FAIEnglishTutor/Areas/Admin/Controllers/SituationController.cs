using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EXE_FAIEnglishTutor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,Admin")]
    public class SituationController : Controller
    {
        private readonly ISituationAdminService _situationService;
        private readonly FaiEnglishContext _context;

        public SituationController(ISituationAdminService situationService, FaiEnglishContext context)
        {
            _situationService = situationService;
            _context = context;
        }

        public async Task<IActionResult> Index(string? search, int? levelId, int? typeId, int page = 1, int pageSize = 10)
        {
            ViewBag.Levels = new SelectList(await _context.Levels.AsNoTracking().ToListAsync(), "LevelId", "LevelName", levelId);
            ViewBag.Types = new SelectList(await _context.Types.AsNoTracking().ToListAsync(), "TypeId", "TypeName", typeId);

            var viewModel = await _situationService.GetPagedAndFilteredAsync(search, levelId, typeId, page, pageSize);
            return View(viewModel);
        }

        private void PopulateDropdowns()
        {
            ViewBag.Types = new SelectList(_context.Types, "TypeId", "TypeName");
            ViewBag.Levels = new SelectList(_context.Levels, "LevelId", "LevelName");
        }

        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new Situation());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Situation situation, IFormFile? imageFile, string[] learningObjectivesList)
        {
            if (ModelState.IsValid)
            {
                // Convert string array to JSON string for LearningObjectives
                if (learningObjectivesList != null && learningObjectivesList.Length > 0)
                {
                    situation.LearningObjectives = JsonSerializer.Serialize(learningObjectivesList.Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
                }

                await _situationService.CreateAsync(situation, imageFile);
                TempData["SuccessMessage"] = "Thêm tình huống thành công!";
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns();
            return View(situation);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var situation = await _situationService.GetByIdAsync(id);
            if (situation == null) return NotFound();

            PopulateDropdowns();
            return View(situation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Situation situation, IFormFile? imageFile, string[] learningObjectivesList)
        {
            if (id != situation.SituatuonId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (learningObjectivesList != null && learningObjectivesList.Length > 0)
                    {
                        situation.LearningObjectives = JsonSerializer.Serialize(learningObjectivesList.Where(x => !string.IsNullOrWhiteSpace(x)).ToList());
                    }
                    else
                    {
                        situation.LearningObjectives = "[]";
                    }

                    await _situationService.UpdateAsync(situation, imageFile);
                    TempData["SuccessMessage"] = "Cập nhật tình huống thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Lỗi đa luồng
                    TempData["ErrorMessage"] = "CẢNH BÁO: Dữ liệu này vừa bị chỉnh sửa bởi một Admin khác! Vui lòng tải lại trang để xem dữ liệu mới nhất trước khi lưu đè.";
                    PopulateDropdowns();
                    return View(situation);
                }
            }
            PopulateDropdowns();
            return View(situation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _situationService.DeleteAsync(id);
            if (success)
                TempData["SuccessMessage"] = "Xóa tình huống thành công!";
            else
                TempData["ErrorMessage"] = "Lỗi khi xóa tình huống.";

            return RedirectToAction(nameof(Index));
        }
    }
}
