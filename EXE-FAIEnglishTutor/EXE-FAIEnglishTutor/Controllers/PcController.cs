using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class PcController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
