using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Mvc;
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
        public HomeController(ILogger<HomeController> logger, ISituationService situationService, IConfiguration configuration)
        {
            _logger = logger;
            _situationService = situationService;
            _configuration = configuration;
            _translatorEndpoint = _configuration["AzureTranslator:Endpoint"] + "translate?api-version=3.0";
            _apiKey = _configuration["AzureTranslator:ApiKey"];
            _region = _configuration["AzureTranslator:Region"];
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

