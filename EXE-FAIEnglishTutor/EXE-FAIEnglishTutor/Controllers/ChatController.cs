using EXE_FAIEnglishTutor.Services.Interface.AI;
using Microsoft.AspNetCore.Mvc;

namespace EXE_FAIEnglishTutor.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatBotService _chatBotService;

        public ChatController(IChatBotService chatBotService)
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

            var response = await _chatBotService.GetChatResponseAsync(message);
            return Json(new { success = true, response });
        }
    }
}
