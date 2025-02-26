using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace EXE_FAIEnglishTutor.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRegisterUserService _registerUserService;

        public AccountController(IUserService userService, IRefreshTokenService refreshTokenService, IRegisterUserService registerUserService)
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _registerUserService = registerUserService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            var model = new LoginModel();

            // Kiểm tra nếu có cookie "SavedEmail" thì lấy giá trị
            if (Request.Cookies.TryGetValue("SavedEmail", out string savedEmail))
            {
                model.Email = savedEmail;
            }

            return View(model);

        }

        //CÒn thiếu phân role
        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu chưa nhập dữ liệu hoặc dữ liệu không hợp lệ, trả về View để hiển thị lỗi
                return View(model);
            }
            var user = _userService.AuthenticateUser(model.Email, model.Password);
            if (user != null)
            {
                var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName ?? ""),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim("PhoneNumber", user.PhoneNumber ?? ""),
                        new Claim("Avatar", user.Avatar ?? ""),
                        new Claim("CreatedAt", user.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                        new Claim("Status", user.Status.ToString()),
                        new Claim("ExpiryDate", user.ExpiryDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "")
                    };
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(claimsIdentity);

                bool isPersistent = model.RememberMe;
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                        new AuthenticationProperties
                        {
                            IsPersistent = isPersistent,
                            ExpiresUtc = isPersistent ? DateTimeOffset.UtcNow.AddDays(1) : DateTimeOffset.UtcNow.AddHours(1) // Thời gian hết hạn của cookie
                        });

                // Xử lý cookie SavedEmail và RefreshToken
                HandleSavedEmailAndRefreshToken(isPersistent, model.Email, user);

                return RedirectToAction("Index", "Home");

            }
            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác";
            return View("Login", model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Xóa cookie RefreshToken
            Response.Cookies.Delete("RefreshToken");

            //CÒN THIẾU ĐÁNH DẤU REFRESHTOKEN KHI LOGOUT

            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {

            return View();

        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await _registerUserService.registerUser(model);
                TempData["RegisterSuccess"] = "success";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đăng ký: {ex.Message}");

                if (ex.Message == "Email gửi thất bại")
                {
                    ViewData["EmailError"] = "Không thể gửi email xác nhận. Vui lòng kiểm tra email của bạn.";
                }
                switch (ex.Message)
                {
                    case "Email exist":
                        ViewData["EmailExist"] = "Email đã tồn tại";
                        break;
                    case "Phone err":
                        ViewData["PhoneErr"] = "Lỗi định dạng số điện thoại";
                        break;
                    case "Phone exist":
                        ViewData["PhoneExist"] = "Số điện thoại đã tồn tại";
                        break;
                }
            }

            return View("Register", model);

        }



        [HttpGet]
        public async Task<IActionResult> ConfirmVerificationToken(string token)
        {
            string result = await _registerUserService.confirmToken(token);


            var messages = new Dictionary<string, string>
            {
                { "Invalid Token", "Invalid Token" },
                { "Token expired", "Your token has expired." },
                { "UserNotFound", "User not found." },
                { "Account Locked", "Your account is locked." },
                { "Account activated", "Your account has been activated" },
                { "Token valid", "Your account has been activated successfully." }
            };
            if (messages.ContainsKey(result))
            {
                ViewData[result.Replace(" ", "")] = messages[result];
            }
            else
            {
                ViewData["UnknownError"] = "An unknown error occurred.";
            }
            return  View("ActivationResult");
        }

        private void HandleSavedEmailAndRefreshToken(bool isPersistent, string email, User user)
        {
            if (!isPersistent)
            {
                // Lưu email trong 7 ngày nếu không chọn Remember Me
                Response.Cookies.Append("SavedEmail", email, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });
            }
            else
            {
                // Xóa cookie SavedEmail nếu đã chọn Remember Me
                Response.Cookies.Delete("SavedEmail");

                // Tạo Refresh Token
                string refreshTokenCode = Guid.NewGuid().ToString();
                var tokenStore = new RefreshToken
                {
                    UserId = user.UserId,
                    TokenCode = refreshTokenCode,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                    DeviceInfo = Request.Headers["User-Agent"].ToString()
                };
                _refreshTokenService.SaveRefreshTokenAsync(tokenStore).GetAwaiter().GetResult();

                // Lưu Refresh Token vào cookie
                Response.Cookies.Append("RefreshToken", refreshTokenCode, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Sử dụng Request.IsHttps nếu chạy HTTPS
                    Expires = DateTime.UtcNow.AddDays(30)
                });
            }
        }

    }
}
