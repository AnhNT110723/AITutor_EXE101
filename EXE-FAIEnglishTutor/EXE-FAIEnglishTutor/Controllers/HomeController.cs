using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Configuration;
using System.Diagnostics;
using System.Security.Claims;
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
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, ISituationService situationService, IConfiguration configuration, IUserService userService)
        {
            _logger = logger;
            _situationService = situationService;
            _configuration = configuration;
            _userService = userService;
            _translatorEndpoint = _configuration["AzureTranslator:Endpoint"] + "translate?api-version=3.0";
            _apiKey = _configuration["AzureTranslator:ApiKey"];
            _region = _configuration["AzureTranslator:Region"];
        }

        [Route("/Mentee")]
        public async Task<IActionResult> Index()
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

