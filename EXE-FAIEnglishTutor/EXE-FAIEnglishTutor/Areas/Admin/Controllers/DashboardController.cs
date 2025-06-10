using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace EXE_FAIEnglishTutor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class DashboardController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DashboardController(IUserService userService, IRoleService roleService, IWebHostEnvironment webHostEnvironment)
        {
            _userService = userService;
            _roleService = roleService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            int totalUsers = await _userService.GetTotalUsersAsync();
            ViewBag.TotalUsers = totalUsers;
            return View();
        }

        [HttpGet("Admin/ListUser")]

        public async Task<IActionResult> ListUser()
        {
            var listUser = await _userService.GetAllUsersAsync();
            return View(listUser.ToList());
        }


        [HttpGet("Admin/EditUser")]
        public async Task<IActionResult> EditUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            var roleId = user.Roles.FirstOrDefault()?.RoleId;

            var UserDto = new Admin_AddUserDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Dob = user.Dob, 
                Password = user.PasswordHash,
                RePassword = user.PasswordHash,
                RoleId = roleId
            };
            var roles = await _roleService.GetAllRole();
            ViewBag.Roles = roles;
            return View("editUser", UserDto);
        }


        [HttpPost("Admin/EditUser")]
        public async Task<IActionResult> EditUser(Admin_AddUserDto model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.Remove("Password");

            }
            ModelState.Remove("RePassword");
            if (!ModelState.IsValid)
            {
                // Trường hợp dữ liệu model không hợp lệ: giữ nguyên dữ liệu và trả lại view
                var roles = await _roleService.GetAllRole();
                ViewBag.Roles = roles;
                return View(model);
            }

            var user = await _userService.GetUserByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();
            }
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = $"{model.CountryCode}{model.PhoneNumber}";
            user.Dob = model.Dob;


            var file = Request.Form.Files.FirstOrDefault(f => f.Name == "Avatar");
            if (file != null && file.Length > 0)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                if (!allowedExtensions.Contains(fileExtension.ToLower()))
                {
                    ModelState.AddModelError("Avatar", "Invalid file type. Only .jpg, .jpeg, .png are allowed.");
                    var roles = await _roleService.GetAllRole();
                    ViewBag.Roles = roles;
                    return View(model);
                }

                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Save relative path to user.Avatar
                user.Avatar = $"/uploads/avatars/{fileName}";
            }
            if (model.RoleId.HasValue)
            {
                var role = await _roleService.GetRoleByIdAsync(model.RoleId.Value);
                if (role != null)
                {
                    user.Roles.Clear();
                    user.Roles.Add(role);
                }
            }

            await _userService.UpdateUser(user);
            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("ListUser");

        }


        [HttpGet("Admin/AddUser")]
        public async Task<IActionResult> AddUser()
        {
            ViewBag.Roles = await _roleService.GetAllRole();
            return View(new Admin_AddUserDto());
        }


        [HttpPost("Admin/AddUser")]
        public async Task<IActionResult> AddUser(Admin_AddUserDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _roleService.GetAllRole();
                return View(model);
            }

            // Tạo user mới
            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = $"{model.CountryCode}{model.PhoneNumber}",
                Dob = model.Dob,
                Status = EXE_FAIEnglishTutor.Enums.AccountStatus.ACTIVATED,
                CreatedAt = DateTime.UtcNow,
                Provider = "Local"
            };

            // Mã hóa mật khẩu (nếu cần)
            if (!string.IsNullOrEmpty(model.Password))
            {
                var passwordHasher = new PasswordHasher<User>();
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
            }

            // Xử lý avatar nếu có
            if (model.Avatar != null && model.Avatar.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/avatars");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Avatar.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Avatar.CopyToAsync(fileStream);
                }

                user.Avatar = "/uploads/avatars/" + uniqueFileName;
            }

            // Xử lý role
            if (model.RoleId.HasValue)
            {
                var role = await _roleService.GetRoleByIdAsync(model.RoleId.Value);
                if (role != null)
                {
                    user.Roles.Add(role);
                }
            }

            await _userService.AddUserAsync(user);
            TempData["Success"] = "Thêm mới thành công!";

            return RedirectToAction("ListUser");
        }



        [HttpGet("Admin/BlockUser/{id}")]
        public async Task<IActionResult> BlockUser(int id)
        {
            await _userService.BlockUserAsync(id);
            TempData["Success"] = "User đã bị block thành công!";
            return RedirectToAction("ListUser");
        }


    }
}
