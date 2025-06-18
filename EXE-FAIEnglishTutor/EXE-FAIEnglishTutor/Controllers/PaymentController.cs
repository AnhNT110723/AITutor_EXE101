using EXE_FAIEnglishTutor.Common;
using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Implementaion;
using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        public PaymentController(IHttpClientFactory httpClientFactory, IPaymentService _aymentService, IUserService userService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _paymentService = _aymentService;
            _userService = userService;
        }
        public async Task<IActionResult> Index()
        {
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null)
            {
               int userId = int.Parse(userIdClaim.Value);
               var user = await _userService.GetUserById(userId);

               return View(user);
            }
            

            return View(new User());
        }


        [HttpPost]
        public async Task<IActionResult> GenerateQR(int planAmount)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException();
            }
            int userId = int.Parse(userIdClaim.Value);

            if (planAmount <= 0)
            {
                return BadRequest("Số tiền không hợp lệ.");
            }
            var paymentDto = new PaymentDto
            {
                TotalAmount = planAmount,
                UserId = userId,
                Content = ""+GenerateUniqueRandomNumber(),
                BankId = Constants.BANK_ID,
                AccountNo = Constants.ACCOUNT_NO,
                AccountName = Constants.ACCOUNT_NAME,
                Status = Constants.PENDING
            };
            string displayContent = $"Thanh toán đơn hàng: {paymentDto.Content}. {Constants.CONTENT}"; 
            // Tạo URL mã QR
            paymentDto.QRCodeUrl = $"https://img.vietqr.io/image/{paymentDto.BankId}-{paymentDto.AccountNo}-qr_only.png?amount={paymentDto.TotalAmount}&addInfo={displayContent}&accountName={paymentDto.AccountName}";

            // Lưu payment vào DB ngay lập tức (trạng thái chờ)
            var paymentEntity = paymentDto.toPayment(); // Chuyển đổi sang entity
            await _paymentService.SavePaymentAsync(paymentEntity);
            paymentDto.PaymentId = paymentEntity.PaymentId;
            return View(paymentDto);
        }
        [HttpGet]
        public async Task<IActionResult> CheckPaymentStatus(int paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return Json(new { success = false, message = "Không tìm thấy giao dịch." });
            }

            try
            {
                var response = await _httpClient.GetAsync("https://script.google.com/macros/s/AKfycbxKGfxCQ5OThSAtTVVZ3HHt1rFwbCa-DZvwsAuL_Oe1eS0XJ9fuUgGazBKJr9Bs2JrEOg/exec");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic data = JsonConvert.DeserializeObject(responseBody);

                var lastPaid = data.data[data.data.Count - 1];
                decimal lastPrice = lastPaid["Giá trị"];
                string lastContent = lastPaid["Mô tả"];

                if (lastPrice >= payment.TotalAmount  && lastContent.Contains(payment.Content))
                {
                    payment.Status = Constants.COMPLETE; // Cập nhật trạng thái
                    await _paymentService.UpdatePaymentAsync(payment);

                    //cần cập nhật trạng thái tk và thời gian hết hạn
                    var user = await _userService.GetUserByIdAsync(payment.UserId);
                    if (user != null)
                    {
                        user.UpgradeLevel = GetUpgradeLevelFromPlan(payment.TotalAmount); // Xác định cấp độ
                        user.ExpiryDate = DateTime.UtcNow.AddMonths(1); // Hết hạn sau 1 tháng
                       await _userService.SaveChangeAsync(user);
                    }

                    return Json(new { success = true, message = "Thanh toán thành công!" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> CancelPayment(int paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return Json(new { success = false, message = "Không tìm thấy giao dịch." });
            }
            payment.Status = Constants.CANCEL;
            await _paymentService.UpdatePaymentAsync(payment);
            return Json(new { success = true });


        }

        private int GetUpgradeLevelFromPlan(decimal planAmount)
        {
            if (planAmount == 2000) return 1; 
            if (planAmount == 200000) return 2; 
            if (planAmount == 640000) return 3;
            return 0; // Không nâng cấp
        }

        private int GenerateUniqueRandomNumber()
        {
            Random random = new Random();
            return random.Next(100000, 1000000);
        }
    }
}
