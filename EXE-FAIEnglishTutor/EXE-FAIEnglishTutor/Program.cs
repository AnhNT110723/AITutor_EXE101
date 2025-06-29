﻿using EXE_FAIEnglishTutor.Configurations;
using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Middleware;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Repositories;
using EXE_FAIEnglishTutor.Services;
using EXE_FAIEnglishTutor.Services.Implementaion.AI;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<SpeechService>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/v1/");
}).AddTypedClient<SpeechService>(client =>
    new SpeechService(client, builder.Configuration["OpenAI:ApiKey"])); // Store API key in appsettings.json

//Thêm dbcontext vào web server
builder.Services.AddDbContextConfiguration(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


//cau hinh view areas
builder.Services.ConfigureRazorViewEngine();

// feat/noitudo
builder.Services.AddHttpClient();


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
        options.AccessDeniedPath = "/Error/401";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;             // Gia hạn cookie nếu có request mới
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"];
        options.ClientSecret = builder.Configuration["Google:ClientSecret"];
        options.CallbackPath = "/login-google";

        // Bắt lỗi nếu login Google thất bại
        options.Events.OnRemoteFailure = context =>
        {
            context.Response.Redirect("/Error/504");
            context.HandleResponse(); // Ngăn chặn lỗi mặc định
            return Task.CompletedTask;
        };
    })
    .AddFacebook(options =>
    {
        options.AppId = builder.Configuration["Facebook:AppId"];
        options.AppSecret = builder.Configuration["Facebook:AppSecret"];
        options.CallbackPath = "/signin-facebook"; // Phải khớp với Redirect URI ở Facebook

        // Thêm các trường thông tin cần lấy từ Facebook
        options.Fields.Add("email");
        options.Fields.Add("name");
        // Lưu access token để có thể gọi API Facebook nếu cần
        options.SaveTokens = true;
    })
    .AddTwitter("Twitter", options =>
    {
        options.ConsumerKey = builder.Configuration["Twitter:APIKey"];
        options.ConsumerSecret = builder.Configuration["Twitter:APIKeySecret"];
        options.CallbackPath = "/signin-twitter";
		options.SaveTokens = true;
	});

var app = builder.Build();

// Thêm middleware xử lý lỗi trạng thái trước các middleware khác
app.UseStatusCodePagesWithRedirects("~/Error/{0}");

app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            // Lấy mã trạng thái từ context
            int statusCode = context.Response.StatusCode;
            context.Response.Redirect($"/Error/{statusCode}");
        });
    });
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseStatusCodePagesWithRedirects("~/Error/{0}"); 
}


//Midleware
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseMiddleware<TokenMiddleware>();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

//Kiem tra area truoc
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=homepage}/{id?}");

app.Run();
