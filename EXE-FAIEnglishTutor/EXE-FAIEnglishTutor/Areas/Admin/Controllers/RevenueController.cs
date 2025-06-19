using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class RevenueController : Controller
    {
        private readonly IUserService _userService;
        private readonly IPaymentService _paymentService;
        public RevenueController(IUserService userService, IPaymentService paymentService )
        {
            _userService = userService;
            _paymentService = paymentService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
