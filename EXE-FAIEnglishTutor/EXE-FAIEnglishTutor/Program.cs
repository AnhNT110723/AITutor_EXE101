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
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;                 // chỉ server có thể truy cập cookie
    options.Cookie.IsEssential = true;              // cookie bắt buộc, dù người dùng có từ chối cookie
});

// Cấu hình Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";         
        options.LogoutPath = "/Account/Logout";         
        options.AccessDeniedPath = "/Account/AccessDenied"; 
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;             // Gia hạn cookie nếu có request mới
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"];
        options.ClientSecret = builder.Configuration["Google:ClientSecret"];
        options.CallbackPath = "/login-google";
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Facebook:AppId"];
        options.AppSecret = builder.Configuration["Facebook:AppSecret"];
        options.CallbackPath = "/signin-facebook"; // Phải khớp với Redirect URI ở Facebook
    })
    .AddTwitter("Twitter", options =>
    {
        options.ConsumerKey = builder.Configuration["Twitter:APIKey"];
        options.ConsumerSecret = builder.Configuration["Twitter:APIKeySecret"];
        options.CallbackPath = "/signin-twitter";
		options.SaveTokens = true;
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
