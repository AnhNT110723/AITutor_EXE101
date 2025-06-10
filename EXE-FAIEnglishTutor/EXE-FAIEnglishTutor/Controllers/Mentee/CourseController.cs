using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers.Mentee
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View("Detail");
        }
        [HttpGet("Course/Detail/TakingAi")] 
        public IActionResult TakingAi()
        {
            return View("TakingAI");
        }
    }


}
