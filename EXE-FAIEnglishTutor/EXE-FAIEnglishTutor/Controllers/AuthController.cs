using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class AuthController : Controller
    {

        private readonly IRegisterUserService _registerUserService;
        public AuthController (IRegisterUserService registerUserService)
        {
            _registerUserService = registerUserService;
        }


        // GET: /Auth/LoginWithGoogle
        public IActionResult LoginWithGoogle(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Callback", "Auth", new { returnUrl }), // nơi middleware chuyển hướng sau khi hoàn tất xác thực.
                IsPersistent = true, 
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) 
            };

            // Thêm tham số prompt để buộc chọn tài khoản
            properties.Items["prompt"] = "select_account"; // Hoặc "login" để bắt nhập lại mật khẩu
            return Challenge(properties, "Google");
        }


        // GET: /Auth/LoginWithFacebook
        [HttpGet]
        public IActionResult LoginWithFacebook(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Callback", "Auth", new { returnUrl }),
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };
            properties.Items["prompt"] = "select_account";
            return Challenge(properties, "Facebook"); 
        }

        // GET: /Auth/LoginWithTwitter
        [HttpGet]
        public IActionResult LoginWithTwitter(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("Callback", "Auth", new { returnUrl }),
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };
            return Challenge(properties, "Twitter"); 
        }

        // GET: /Auth/Callback (Callback chung cho tất cả external login)
        [HttpGet]
        public async Task<IActionResult> Callback(string returnUrl = "/")
        {
            // Kiểm tra xem người dùng đã được xác thực chưa
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var providerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Xác định provider dựa trên Authentication Scheme
            string provider = "Unknown"; // Mặc định là Unknown

            // Kiểm tra từng scheme (Google, Facebook, v.v.)
            var googleResult = await HttpContext.AuthenticateAsync("Google");
            if (googleResult?.Principal != null && googleResult.Succeeded)
            {
                provider = "Google";
            }

            var facebookResult = await HttpContext.AuthenticateAsync("Facebook");
            if (facebookResult?.Principal != null && facebookResult.Succeeded)
            {
                provider = "Facebook";
            }

            var twitterResult = await HttpContext.AuthenticateAsync("Twitter");
            if (twitterResult?.Principal != null && twitterResult.Succeeded)
            {
                provider = "Twitter";
            }

            // Nếu không xác định được, thử lấy từ claim (dành cho Google/Facebook)
            if (provider == "Unknown")
            {
                provider = User.FindFirst("urn:google:provider")?.Value
                    ?? User.FindFirst("urn:facebook:provider")?.Value
                    ?? User.FindFirst("urn:twitter:provider")?.Value
                    ?? "Unknown";
            }

            //Lưu db nếu đang nhập lần đầu
            try
            {
                var user = await _registerUserService.FindOrCreateExternalUserAsync(email, name, provider, providerId);
            }
            catch (InvalidOperationException ex)
            {
                
                TempData["Error"] = ex.Message;
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account"); 
            }

            return Redirect(returnUrl); // Quay lại trang yêu cầu
        }


    }
}
