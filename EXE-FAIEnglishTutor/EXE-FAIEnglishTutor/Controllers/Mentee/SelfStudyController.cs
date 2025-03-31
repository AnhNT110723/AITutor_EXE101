using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers.Mentee
{
    public class SelfStudyController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
