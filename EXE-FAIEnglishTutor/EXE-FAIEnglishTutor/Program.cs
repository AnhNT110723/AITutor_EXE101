using EXE_FAIEnglishTutor.Configurations;
using EXE_FAIEnglishTutor.Middleware;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories;
using EXE_FAIEnglishTutor.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Thêm dbcontext vào web server
builder.Services.AddDbContextConfiguration(builder.Configuration);
// Cấu hình DI
builder.Services.AddDependencyInjectionConfiguration();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian hết hạn session
    options.Cookie.HttpOnly = true;                 // chỉ server có thể truy cập cookie
    options.Cookie.IsEssential = true;              // cookie bắt buộc, dù người dùng có từ chối cookie
});

// Cấu hình Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";         // Đường dẫn trang đăng nhập
        options.LogoutPath = "/Account/Logout";         // Đường dẫn đăng xuất
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn khi không có quyền truy cập
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Thời gian hết hạn cookie
        options.SlidingExpiration = true;             // Gia hạn cookie nếu có request mới
        //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
        //options.Cookie.HttpOnly = true; // Chống XSS
        //options.Cookie.SameSite = SameSiteMode.Strict; // Chống CSRF cross-site
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}




//Midleware
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseMiddleware<TokenMiddleware>();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
