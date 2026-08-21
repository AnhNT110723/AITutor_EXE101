using EXE_FAIEnglishTutor.Services.Interface.AI;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using EXE_FAIEnglishTutor.Common;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class ListeningAIController : Controller
    {
        private readonly IAIService _aiService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ISituationService _situationService;
        private readonly IUserService _userService;
        private readonly ISpeakingAIService _speakingAiService;
        private readonly AzureTranslatorConfig _azureTranslatorConfig;
        private readonly HttpClient _httpClient;
        public ListeningAIController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IAIService aiService, ISituationService situationService, IUserService userService, ISpeakingAIService speakingAIService, IOptions<AzureTranslatorConfig> azureTranslatorConfig
        )
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _aiService = aiService;
            _situationService = situationService;
            _userService = userService;
            _speakingAiService = speakingAIService;
            _azureTranslatorConfig = azureTranslatorConfig.Value;
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpGet("Mentee/Listening")]
        public async Task<IActionResult> GetListSituationsAsync2()
        {
            var levels = await _situationService.GetAllLevelAsync();
            var listSituations = await _situationService.GetListSituationByRolePlay(Constants.LISTENING);
            ViewBag.levels = levels;

            return View("ListSituation", listSituations);
        }

        [HttpGet("Mentee/Listening/ListPartial")]       
        public async Task<IActionResult> GetListSituationsPartialAsync(string keyword = "", string category = "")
        {
            try
            {

                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword.Trim();
                category = string.IsNullOrWhiteSpace(category) ? "" : category.Trim();


                var listSituations = await _situationService.GetListSituationByRolePlay(Constants.LISTENING, keyword, category);

                // Ch?n ch? các thu?c tính c?n thi?t
                var result = listSituations.Select(s => new
                {
                    situationId = s.SituatuonId,
                    situationName = s.SituationName,
                    imageUrl = s.ImageUrl,
                    level = new
                    {
                        levelName = s.Level?.LevelName ?? "Unknown"
                    }
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                // Ghi log l?i n?u c?n (tùy thu?c vào h? th?ng logging c?a b?n)
                return StatusCode(500, new { error = "Ðã x?y ra l?i khi t?i danh sách tình hu?ng." });
            }
        }

    }
}

