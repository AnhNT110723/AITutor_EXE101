using EXE_FAIEnglishTutor.Services.Interface.AI;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using EXE_FAIEnglishTutor.Common;
using System.Security.Claims;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class ListeningAIController : Controller
    {
        private readonly IOpenAIService _aiService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ISituationService _situationService;
        private readonly IUserService _userService;
        private readonly ISpeakingAIService _speakingAiService;
        private readonly AzureTranslatorConfig _azureTranslatorConfig;
        private readonly HttpClient _httpClient;
        public ListeningAIController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IOpenAIService aiService, ISituationService situationService, IUserService userService, ISpeakingAIService speakingAIService, IOptions<AzureTranslatorConfig> azureTranslatorConfig
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim.Value);
                var user = await _userService.GetUserById(userId);
                if (user.ExpiryDate != null && user.UpgradeLevel > 0)
                {
                    ViewBag.userCheckMember = true;
                }
            }


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

                // Chọn chỉ các thuộc tính cần thiết
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
                // Ghi log lỗi nếu cần (tùy thuộc vào hệ thống logging của bạn)
                return StatusCode(500, new { error = "Đã xảy ra lỗi khi tải danh sách tình huống." });
            }
        }

    }
}
