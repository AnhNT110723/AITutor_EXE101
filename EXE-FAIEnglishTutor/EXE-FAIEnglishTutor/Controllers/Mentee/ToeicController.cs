using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers.Mentee
{
    public class ToeicController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Toeic/TestOnline")]
        public IActionResult TestOnline()
        {
            return View("TestOnline");
        }


        [HttpGet("Toeic/TestOnline/TestInput")]
        public IActionResult TestInput()
        {
            return View("TestInput");
        }


        [HttpGet("Toeic/TestOnline/TestInput/TestToeic")]
        public IActionResult TestToeic()
        {
            return View("TestToeic");
        }

    }
}
