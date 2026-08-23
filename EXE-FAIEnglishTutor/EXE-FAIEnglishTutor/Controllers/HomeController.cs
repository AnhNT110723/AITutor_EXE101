using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;
using System.Text;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISituationService _situationService;
        private readonly IConfiguration _configuration;
        private readonly string _translatorEndpoint;
        private readonly string _apiKey;
        private readonly string _region;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public HomeController(ILogger<HomeController> logger, ISituationService situationService, IConfiguration configuration, IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            _situationService = situationService;
            _configuration = configuration;
            _localizer = localizer;
            _translatorEndpoint = _configuration["AzureTranslator:Endpoint"] + "translate?api-version=3.0";
            _apiKey = _configuration["AzureTranslator:ApiKey"];
            _region = _configuration["AzureTranslator:Region"];
        }

        [Route("/debug-locale")]
        public IActionResult DebugLocale()
        {
            var feature = HttpContext.Features.Get<IRequestCultureFeature>();
            var culture = feature?.RequestCulture.Culture.Name ?? "null";
            var uiCulture = feature?.RequestCulture.UICulture.Name ?? "null";
            var testKey = _localizer["NewlyReleased"];
            var testKey2 = _localizer["RolePlay"];
            return Content($"Culture={culture} | UICulture={uiCulture} | NewlyReleased='{testKey.Value}' | RolePlay='{testKey2.Value}' | ResourceNotFound={testKey.ResourceNotFound}");
        }

        [Route("/Mentee")]
        public async Task<IActionResult> Index()
        {
            var situations = await _situationService.GetAllSituation();
            return View(situations);
        }


        public IActionResult homePage()
        {

            return View("homepage");
        }

    public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

