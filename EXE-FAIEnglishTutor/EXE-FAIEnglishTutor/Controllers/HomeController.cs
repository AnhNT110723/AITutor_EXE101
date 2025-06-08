using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISituationService _situationService;

        public HomeController(ILogger<HomeController> logger, ISituationService situationService)
        {
            _logger = logger;
            _situationService = situationService;
        }

        public async Task<IActionResult> Index()
        {
            var situations = await _situationService.GetAllSituation();
            return View(situations);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult LearningAI()
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

