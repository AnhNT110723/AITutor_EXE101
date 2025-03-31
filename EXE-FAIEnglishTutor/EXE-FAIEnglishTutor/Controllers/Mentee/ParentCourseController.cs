using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers.Mentee
{
    public class ParentCourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Detail()
        {
            return View("Detail");
        }

    }
}
