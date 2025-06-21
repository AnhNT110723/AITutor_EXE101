using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Tweetinvi;
namespace EXE_FAIEnglishTutor.Controllers
{
    public class AuthController : Controller
    {

        private readonly IRegisterUserService _registerUserService;
		private readonly IConfiguration _configuration;
		public AuthController (IRegisterUserService registerUserService, IConfiguration configuration)
        {
            _registerUserService = registerUserService;
			_configuration = configuration; // Thêm để lấy Consumer Key/Secret
		}


		// GET: /Auth/LoginWithGoogle
        public IActionResult LoginWithGoogle(string returnUrl = "/Mentee")
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
        public IActionResult LoginWithFacebook(string returnUrl = "/Mentee")
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
        public IActionResult LoginWithTwitter(string returnUrl = "/Mentee")
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
		public async Task<IActionResult> Callback(string returnUrl = "/Mentee")
		{
			// Kiểm tra xem người dùng đã được xác thực chưa
			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Login", "Account");
			}
			var (provider, email, name, providerId) = await DetermineProviderAndUserInfo();
			if (provider == "Unknown")
			{
				TempData["Error"] = "Không xác định được provider.";
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				return RedirectToAction("Login", "Account");
			}

			if (email == null)
			{
				TempData["Error"] = "Không lấy được email from ..." + provider;
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				return RedirectToAction("Login", "Account");
			}
			try
			{
				await RegisterAndSignInUser(email, name, provider, providerId);
			}
			catch (InvalidOperationException ex)
			{

				TempData["Error"] = ex.Message;
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				return RedirectToAction("Login", "Account");
			}
			return Redirect(returnUrl);
		}



		private async Task<(string provider, string email, string name, string providerId)> DetermineProviderAndUserInfo()
		{
			string provider = "Unknown";
			string email = User.FindFirst(ClaimTypes.Email)?.Value;
			string name = User.FindFirst(ClaimTypes.Name)?.Value;
			string providerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
				email ??= await FetchTwitterEmail(twitterResult, email);
			}

			// Fallback nếu provider vẫn chưa xác định
			if (provider == "Unknown")
			{
				provider = User.FindFirst("urn:google:provider")?.Value
					?? User.FindFirst("urn:facebook:provider")?.Value
					?? User.FindFirst("urn:twitter:provider")?.Value
					?? "Unknown";
			}

			return (provider, email, name, providerId);
		}

		private async Task<string> FetchTwitterEmail(AuthenticateResult twitterResult, string currentEmail)
		{
			if (!string.IsNullOrEmpty(currentEmail))
			{
				return currentEmail; // Nếu đã có email, không cần gọi API
			}

			try
			{
				// Debug Items
				var items = twitterResult.Properties.Items;
				if (items != null)
				{
					var debugInfo = string.Join(", ", items.Select(kvp => $"{kvp.Key}: {kvp.Value}"));
					TempData["Debug"] = $"Items content: {debugInfo}";
				}

				var accessToken = twitterResult.Properties.Items[".Token.access_token"];
				var accessTokenSecret = twitterResult.Properties.Items[".Token.access_token_secret"];
				var consumerKey = _configuration["Twitter:APIKey"];
				var consumerSecret = _configuration["Twitter:APIKeySecret"];

				if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(accessTokenSecret))
				{
					TempData["Error"] = "Không tìm thấy oauth_token hoặc oauth_token_secret.";
					return currentEmail;
				}

				var twitterClient = new TwitterClient(consumerKey, consumerSecret, accessToken, accessTokenSecret);
				var user = await twitterClient.Users.GetAuthenticatedUserAsync();
				return user.Email ?? currentEmail; // Lấy email hoặc giữ nguyên nếu null
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Không thể lấy email từ Twitter: {ex.Message}";
				return currentEmail; // Trả về email hiện tại nếu lỗi
			}
		}

		private async Task RegisterAndSignInUser(string email, string name, string provider, string providerId)
		{
			try
			{
				var user = await _registerUserService.FindOrCreateExternalUserAsync(email, name, provider, providerId);

				var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
				new Claim(ClaimTypes.Name, user.FullName ?? ""),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim("PhoneNumber", user.PhoneNumber ?? ""),
				new Claim("Avatar", user.Avatar ?? ""),
				new Claim("CreatedAt", user.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
				new Claim("Status", user.Status.ToString()),
				new Claim("ExpiryDate", user.ExpiryDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
				new Claim("Provider", provider)
			};

				foreach (var role in user.Roles)
				{
					claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
				}

				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				var principal = new ClaimsPrincipal(claimsIdentity);

				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
					new AuthenticationProperties
					{
						IsPersistent = true,
						ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
					});
			}
			catch (InvalidOperationException ex)
			{
				TempData["Error"] = ex.Message;
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				throw; // Ném lại để Callback xử lý redirect
			}
		}
	}
}
