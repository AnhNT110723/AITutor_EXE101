using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        public ProfileController (IProfileService profileService)
        {
            _profileService = profileService;
        }


        //GET: Profile/Index
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized(); // hoặc chuyển hướng login
            }
            int userId = int.Parse(userIdClaim.Value);
            ProfileDto user = await _profileService.GetUserById(userId);
            ViewBag.RegionCode = user.CountryCode.ToLower();
            return View(user);
        }
    }
}
