using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class testController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
