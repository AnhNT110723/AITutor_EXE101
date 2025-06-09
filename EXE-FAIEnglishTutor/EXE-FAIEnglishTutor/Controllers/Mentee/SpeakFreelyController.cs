using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers.Mentee
{
	public class SpeakFreelyController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

        public IActionResult BotFreeTalk()	
        {
            return View("BotFreeTalk");
        }
    }
}
