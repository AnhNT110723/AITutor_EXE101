using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using EXE_FAIEnglishTutor.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
namespace EXE_FAIEnglishTutor.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IRegisterUserService _registerUserService;
        private readonly IForgotPasswordService _forgotPasswordService;
        private readonly IVerificationTokenService _verificationTokenService;

        public AccountController(IUserService userService, IRefreshTokenService refreshTokenService, IRegisterUserService registerUserService,
            IForgotPasswordService forgotPasswordService, IVerificationTokenService verificationTokenService)
        {
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _registerUserService = registerUserService;
            _forgotPasswordService = forgotPasswordService;
            _verificationTokenService = verificationTokenService;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new LoginDto();

            // Kiểm tra nếu có cookie "SavedEmail" thì lấy giá trị
            if (Request.Cookies.TryGetValue("SavedEmail", out string savedEmail))
            {
                model.Email = savedEmail;
            }

            return View(model);

        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                // Nếu chưa nhập dữ liệu hoặc dữ liệu không hợp lệ, trả về View để hiển thị lỗi
                return View(model);
            }
            var user = _userService.AuthenticateUser(model.Email, model.Password);
            if (user != null)
            {

                // Gọi hàm kiểm tra trạng thái tài khoản
                if (HandleAccountStatus(user.Status))
                {
                    return View(model);
                }

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
            ViewBag.ErrorMessage = "Incorrect username or password";
            return View("Login", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    ViewData["EmailError"] = "The confirmation email could not be sent. Please check your email.";
                }
                switch (ex.Message)
                {
                    case "Email exist":
                        ViewData["EmailExist"] = "Email already exists";
                        break;
                    case "Phone err":
                        ViewData["PhoneErr"] = "Phone number format error";
                        break;
                    case "Phone exist":
                        ViewData["PhoneExist"] = "Phone number already exists";
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
            return View("ActivationResult");
        }

        // POST: /Account/ReSendActivation
        [HttpPost]
        public async Task<IActionResult> ReSendActivation([FromForm] string email)
        {
            if (string.IsNullOrEmpty(email) || !new EmailAddressAttribute().IsValid(email))
            {
                return Json(new { success = false, message = "Định dạng email không hợp lệ." });
            }

            try
            {
                await _registerUserService.resendActivation(email);
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "NotFoundTokenByUserId":
                        return Json(new { success = false, message = "This email is not registered!" });
                    case "NotFound":
                        return Json(new { success = false, message = "Your account not found!" });
                    case "Account Locked":
                        return Json(new { success = false, message = "Your account has been locked!" });
                    case "Account activated":
                        return Json(new { success = false, message = "Your account has been activated!" });
                }
            }
            return Json(new { success = true, message = "Activation mail has been sent successfully, please check your mail and activate your account!" });
        }

        //GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword(string email)
        {
            return View();
        }

        //Post: /Account/ForgotPassword
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _forgotPasswordService.ForGotPassword(model.Email);
                TempData["SendLinkResendPWSuccess"] = "success";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "NotFoundAccount":
                        ViewData["NotFoundAccount"] = "Your email is not registered yet.";
                        break;
                    case "Account Locked":
                        ViewData["AccountLocked"] = "Your account has been locked, please contact customer service for assistance.";
                        break;
                    case "Account not activate":
                        ViewData["AccountNotActivate"] = "Your account is not activated yet.";
                        break; 
                    case "NotFoundTokenByUserId":
                        ViewData["NotFoundTokenByUserId"] = "Your account is not registered locally, so you cannot use this function. Thank you.";
                        break;
                }
            }


            return View(model);
        }


        // GET: /Account/ResetPassword
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            string result = await _forgotPasswordService.CheckTokenValid(token);
            var messages = new Dictionary<string, string>
            {
                { "Invalid token", "Invalid Token" },
                { "Token expired", "Your token has expired." },
                //{ "UserNotFound", "User not found." },
                //{ "Account Locked", "Your account is locked." },
                //{ "Account activated", "Your account has been activated" },
                { "Token Valid", "true" }
            };
            if (messages.ContainsKey(result))
            {
                ViewData[result.Replace(" ", "")] = messages[result];
            }
            else
            {
                ViewData["UnknownError"] = "An unknown error occurred.";
            }
            return View("ResetPassword");
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewData["TokenValid"] = "true";
                return View(model);
            }
            //Lấy token của reset pass nhung dùng chung với vetification token
            VerificationToken vertificationToken = await _verificationTokenService.getVerificationToken(model.Token);
            string email = vertificationToken.User.Email;
            if (email == null)
            {
                ViewBag.EmailNull = true;
                ViewData["TokenValid"] = "true";
                return View();
            }
            await _forgotPasswordService.ResetPassword(email, model.Password, model.RePassword);
            TempData["Success"] = "True";
            ViewData["TokenValid"] = "true";
            return View();

        }





        private bool HandleAccountStatus(AccountStatus status)
        {
            if (status == AccountStatus.LOCKED)
            {
                ViewBag.ErrorLockedAcc = "Your account has been locked.";
                return true; // Trả về true để báo hiệu cần dừng xử lý
            }
            if (status == AccountStatus.PENDING)
            {
                ViewBag.ErrorPendingAcc = "Your account is not activated yet.";
                return true; // Trả về true để báo hiệu cần dừng xử lý
            }
            return false; // Tài khoản hợp lệ, tiếp tục xử lý
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
