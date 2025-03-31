using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers.Mentee
{
    public class IeltsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Ielts/TestOnline")]
        public IActionResult TestOnline()
        {
            return View("TestOnline");
        }


        [HttpGet("Ielts/TestOnline/TestIelts1")]
        public IActionResult TestIelts1()
        {
            return View("TestIelts1");
        }


        [HttpGet("Ielts/TestOnline/TestIelts2")]
        public IActionResult TestIelts2()
        {
            return View("TestIelts2");
        }   

        [HttpGet("Ielts/TestOnline/ResultTest")]
        public IActionResult ResulTestIelts()
        {
            return View("ResultTest");
        }
    }
}
