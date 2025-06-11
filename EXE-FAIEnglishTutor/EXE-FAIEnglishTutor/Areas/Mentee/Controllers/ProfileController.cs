using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tweetinvi.Core.Models;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }


        //GET: Profile/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await LoadUserProfileAsync();
                return View(user);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch
            {
                return StatusCode(500);
            }

        }

        //POST Profile/SaveChangeAsync
        [HttpPost]
        public async Task<IActionResult> SaveChangeAsync(ProfileDto model)
        {

            // Bỏ qua lỗi [Required] của Password và RePassword nếu để trống
            if (string.IsNullOrEmpty(model.OldPassword) && string.IsNullOrEmpty(model.Password) && string.IsNullOrEmpty(model.RePassword))
            {
                ModelState.Remove("OldPassword");
                ModelState.Remove("Password");
                ModelState.Remove("RePassword");
            }

            if (!ModelState.IsValid)
            {
                return await ReturnProfileViewAsync(model);
            }

            try
            {
                var user = await _profileService.SaveChangeAsync(model);
                // Lưu avatar mới vào cookie
                if (!string.IsNullOrEmpty(model.AvatarStr) || model.Avatar != null)
                {
                    var identity = (ClaimsIdentity)User.Identity;
                    var avatarClaim = identity.FindFirst("Avatar");
                    if (avatarClaim != null)
                    {
                        identity.RemoveClaim(avatarClaim);
                    }
                    identity.AddClaim(new Claim("Avatar", user.Avatar));

                    // Cập nhật lại cookie sau khi thay đổi avatar
                    var claimsPrincipal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                }
                TempData["success"] = "Save information change successfully!";
                return RedirectToAction("Index");

            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (ApplicationException)
            {
                return StatusCode(500);
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "Old pass is wrong":
                        TempData["OldPassErr"] = "The old password is not correct!";
                        break;
                    case "Phone err":
                        TempData["PhoneErr"] = "The phone number is invalid!";
                        break;
                    case "Phone Exist":
                        TempData["PhoneExist"] = "The phone number is exist!";
                        break;

                }
                return await ReturnProfileViewAsync(model);
            }


        }

        // Hàm tiện ích để trả về view với fullModel
        private async Task<IActionResult> ReturnProfileViewAsync(ProfileDto model)
        {
            try
            {
                var fullModel = await LoadUserProfileAsync();
                fullModel.FullName = model.FullName;
                fullModel.Dob = model.Dob;
                fullModel.Phone = model.Phone;
                fullModel.Province = model.Province;
                fullModel.CountryCode = model.CountryCode?.ToLower();
                fullModel.OldPassword = model.OldPassword;
                fullModel.Password = model.Password;
                fullModel.RePassword = model.RePassword;
                ViewBag.RegionCode = model.CountryCode?.ToLower();
                return View("Index", fullModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading your profile.";
                return View("Index", model ?? new ProfileDto());
            }
        }

        [HttpGet("api/provinces")]
        public async Task<IActionResult> GetProvinces()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://provinces.open-api.vn/api/p/");
                var data = await response.Content.ReadAsStringAsync();
                return Content(data, "application/json");
            }
        }
        private async Task<ProfileDto> LoadUserProfileAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException();
            }

            int userId = int.Parse(userIdClaim.Value);
            var user = await _profileService.GetUserById(userId);
            ViewBag.RegionCode = user.CountryCode?.ToLower();

            return user;
        }

    }
}