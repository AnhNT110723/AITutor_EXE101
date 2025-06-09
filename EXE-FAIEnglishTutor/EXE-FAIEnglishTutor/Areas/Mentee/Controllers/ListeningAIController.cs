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
            var levels = await _situationService.GetAllLevelAsync();
            var listSituations = await _situationService.GetListSituationByRolePlay(Constants.LISTENING);
            ViewBag.levels = levels;

            return View("ListSituation", listSituations);
        }

    }
}
