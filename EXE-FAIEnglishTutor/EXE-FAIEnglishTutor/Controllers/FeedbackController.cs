using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(IConfiguration configuration, HttpClient httpClient, ILogger<FeedbackController> logger)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _logger = logger;
        }

        public class FeedbackModel
        {
            public string Name { get; set; }
            public string Rating { get; set; }
            public string Category { get; set; }
            public string Message { get; set; }
        }

        [HttpPost]
        [Route("Feedback/Submit")]
        public async Task<IActionResult> Submit([FromBody] FeedbackModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Message))
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                var webhookUrl = _configuration["GoogleSheets:WebhookUrl"];
                if (string.IsNullOrEmpty(webhookUrl))
                {
                    // Nếu chưa cấu hình webhook, chỉ log ra console tạm thời
                    _logger.LogInformation("[Feedback Received] {Name} - {Category}: {Message}", model.Name, model.Category, model.Message);
                    return Ok();
                }

                // Gửi dữ liệu tới Google Apps Script Webhook
                var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(webhookUrl, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    return Ok();
                }

                return StatusCode(500, "Lỗi khi lưu dữ liệu lên Google Sheets.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý feedback");
                return StatusCode(500, "Lỗi hệ thống.");
            }
        }
    }
}
