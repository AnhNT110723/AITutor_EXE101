using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class ToeicController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpGet("Mentee/Toeic/TestOnline")]
        public IActionResult TestOnline()
        {
            return View("TestOnline");
        }


        [HttpGet("Mentee/Toeic/TestOnline/TestInput")]
        public IActionResult TestInput()
        {
            return View("TestInput");
        }


        [HttpGet("Mentee/Toeic/TestOnline/TestInput/TestToeic")]
        public IActionResult TestToeic()
        {
            return View("TestToeic");
        }
    }
}
