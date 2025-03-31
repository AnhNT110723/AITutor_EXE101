using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers.Mentee
{
	public class ProfileController : Controller
	{
		public IActionResult Index()
		{
			return View("Profile");
		}
	}
}
