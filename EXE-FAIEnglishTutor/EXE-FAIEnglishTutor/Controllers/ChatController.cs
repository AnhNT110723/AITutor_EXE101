using EXE_FAIEnglishTutor.Services.Interface.AI;
using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class ChatController : Controller
    {
        private readonly ISpeakingAIService _chatBotService;

        public ChatController(ISpeakingAIService chatBotService)
        {
            _chatBotService = chatBotService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return Json(new { success = false, response = "Vui lòng nhập tin nhắn!" });
            }
            try
            {
                var messages = new[]
                {
                    new { role = "user", content = message }
                };
                var response = await _chatBotService.GetChatResponseAsync(messages);
                return Json(new { success = true, response });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, response = $"Lỗi kết nối AI: {ex.Message}" });
            }
        }
    }
}
