using EXE_FAIEnglishTutor.Common;
using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Implementaion;
using EXE_FAIEnglishTutor.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        private readonly PayOSClient _payOS;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IHttpClientFactory httpClientFactory, IPaymentService _aymentService, IUserService userService, PayOSClient payOS, ILogger<PaymentController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _paymentService = _aymentService;
            _userService = userService;
            _payOS = payOS;
            _logger = logger;
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
            if (userIdClaim == null) throw new UnauthorizedAccessException();
            int userId = int.Parse(userIdClaim.Value);

            if (planAmount <= 0) return BadRequest("Số tiền không hợp lệ.");

            // Sinh OrderCode độc nhất (PayOS yêu cầu orderCode phải là số nguyên int53, tối đa 9007199254740991)
            long orderCode = long.Parse(DateTime.Now.ToString("yyMMddHHmmss"));

            var paymentDto = new PaymentDto
            {
                TotalAmount = planAmount,
                UserId = userId,
                Content = orderCode.ToString(),
                BankId = Constants.BANK_ID,
                AccountNo = Constants.ACCOUNT_NO,
                AccountName = Constants.ACCOUNT_NAME,
                Status = Constants.PENDING
            };

            // Lưu payment vào DB ngay lập tức (trạng thái chờ)
            var paymentEntity = paymentDto.toPayment();
            await _paymentService.SavePaymentAsync(paymentEntity);

            /* --- BẮT ĐẦU CODE PAYOS --- */
            try 
            {
                // Đường dẫn trả về khi thanh toán xong hoặc hủy
                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                string returnUrl = $"{baseUrl}/"; 
                string cancelUrl = $"{baseUrl}/Payment/Index";

                var paymentRequest = new CreatePaymentLinkRequest 
                {
                    OrderCode = orderCode,
                    Amount = (int)planAmount,
                    Description = "Donate FAI English", 
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                };

                var createPayment = await _payOS.PaymentRequests.CreateAsync(paymentRequest);
                // Trả về view GenerateQR của riêng chúng ta thay vì Redirect sang PayOS
                paymentDto.QRCodeUrl = $"https://img.vietqr.io/image/{createPayment.Bin}-{createPayment.AccountNumber}-qr_only.png?amount={createPayment.Amount}&addInfo={createPayment.Description}&accountName={createPayment.AccountName}";
                paymentDto.PaymentId = paymentEntity.PaymentId;

                return View("GenerateQR", paymentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo link thanh toán PayOS cho OrderCode: {OrderCode}", orderCode);
                return BadRequest("Không thể tạo link thanh toán PayOS: " + ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> CheckPaymentStatus(int paymentId)
        {
            // CÁCH MỚI: Chỉ check Local DB, không gọi qua API Google Sheet nữa
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null)
            {
                return Json(new { success = false, message = "Không tìm thấy giao dịch." });
            }

            if (payment.Status == Constants.COMPLETE)
            {
                return Json(new { success = true, message = "Thanh toán thành công!" });
            }

            return Json(new { success = false });
        }

        [HttpPost("/Payment/PayOSWebhook")]
        public async Task<IActionResult> PayOSWebhook([FromBody] Webhook body)
        {
            try
            {
                _logger.LogInformation("Nhận Webhook từ PayOS. Code: {Code}, Desc: {Desc}", body.Code, body.Description);
                
                // Xác thực chữ ký xem có đúng là PayOS gửi không
                var data = await _payOS.Webhooks.VerifyAsync(body);

                if (data == null)
                {
                    _logger.LogWarning("Xác thực Webhook PayOS thất bại. Dữ liệu rỗng.");
                    return Ok(new { success = false, message = "Xác thực thất bại" });
                }

                // Lấy orderCode ra, tìm trong DB
                string orderCode = data.OrderCode.ToString();
                _logger.LogInformation("Xác thực Webhook thành công. OrderCode: {OrderCode}", orderCode);

                // Tìm theo Content vì chúng ta lưu orderCode vào trường Content
                var payment = await _paymentService.GetPaymentByContentAsync(orderCode); 

                if (payment != null && payment.Status != Constants.COMPLETE)
                {
                    _logger.LogInformation("Tìm thấy giao dịch {OrderCode} hợp lệ. Tiến hành cập nhật trạng thái...", orderCode);
                    // Cập nhật trạng thái Payment
                    payment.Status = Constants.COMPLETE;
                    await _paymentService.UpdatePaymentAsync(payment);

                    // Cập nhật User
                    var user = await _userService.GetUserByIdAsync(payment.UserId);
                    if (user != null)
                    {
                        user.UpgradeLevel = GetUpgradeLevelFromPlan(payment.TotalAmount);
                        user.ExpiryDate = DateTime.UtcNow.AddMonths(1);
                        await _userService.SaveChangeAsync(user);
                        _logger.LogInformation("Đã nâng cấp VIP thành công cho User: {UserId}, Cấp độ: {UpgradeLevel}", user.UserId, user.UpgradeLevel);
                    }
                    else
                    {
                        _logger.LogWarning("Không tìm thấy thông tin User: {UserId} để nâng cấp VIP.", payment.UserId);
                    }
                }
                else if (payment == null)
                {
                    _logger.LogWarning("Không tìm thấy giao dịch nào có OrderCode (Content): {OrderCode} trong Database.", orderCode);
                }
                else 
                {
                    _logger.LogInformation("Giao dịch {OrderCode} đã ở trạng thái COMPLETE từ trước.", orderCode);
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi nghiêm trọng khi xử lý Webhook từ PayOS.");
                return Ok(new { success = false, message = ex.Message });
            }
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
            if (planAmount == Constants.MEMBER) return 1; 
            if (planAmount == Constants.VIP) return 2; 
            if (planAmount == Constants.ADVANCE) return 3;
            return 0; // Không nâng cấp
        }

        private int GenerateUniqueRandomNumber()
        {
            Random random = new Random();
            return random.Next(100000, 1000000);
        }
    }
}


/* --- CODE CŨ (Dùng Google Sheet) ---
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
                   payment.Status = Constants.COMPLETE; 
                   await _paymentService.UpdatePaymentAsync(payment);

                   var user = await _userService.GetUserByIdAsync(payment.UserId);
                   if (user != null)
                   {
                       user.UpgradeLevel = GetUpgradeLevelFromPlan(payment.TotalAmount); 
                       user.ExpiryDate = DateTime.UtcNow.AddMonths(1); 
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
           ---------------------------------------*/