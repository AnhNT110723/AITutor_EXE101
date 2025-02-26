using EXE_FAIEnglishTutor.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EXE_FAIEnglishTutor.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TokenMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    var refreshToken = context.Request.Cookies["RefreshToken"];

                    if (refreshToken != null)
                    {
                        // Tạo scope mới
                        using (var scope = _serviceScopeFactory.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<FaiEnglishContext>();

                            var token = await dbContext.RefreshTokens
                                          .Include(t => t.User) // Load dữ liệu User từ DB
                                          .ThenInclude(u => u.Roles) // Load luôn danh sách Roles nếu có
                                          .FirstOrDefaultAsync(t => t.TokenCode == refreshToken);

                            if (token != null && token.ExpiryDate > DateTime.UtcNow)
                            {
                                var user = token.User;
                                if (user != null)
                                {
                                    var claims = new List<Claim> {

                            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                            new Claim(ClaimTypes.Name, user.FullName ?? ""),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim("PhoneNumber", user.PhoneNumber ?? ""),
                            new Claim("Avatar", user.Avatar ?? ""),
                            new Claim("CreatedAt", user.CreatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),
                            new Claim("Status", user.Status.ToString()),
                            new Claim("ExpiryDate", user.ExpiryDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "")
                            };
                                    // Thêm tất cả các role của user vào claims
                                    foreach (var role in user.Roles)
                                    {
                                        claims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                                    }

                                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                    var authProperties = new AuthenticationProperties
                                    {
                                        IsPersistent = true,
                                        ExpiresUtc = DateTime.UtcNow.AddDays(30)
                                    };


                                    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                              new ClaimsPrincipal(claimsIdentity), authProperties);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                // Ví dụ: sử dụng ILogger hoặc ghi ra console
                Console.WriteLine("TokenMiddleware error: " + ex.Message);
            }
            await _next(context);
        }
    }
}
