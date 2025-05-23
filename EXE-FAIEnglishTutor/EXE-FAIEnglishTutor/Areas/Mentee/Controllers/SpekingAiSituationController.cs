using EXE_FAIEnglishTutor.Common;
using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Implimentaion.AI;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class SpekingAiSituationController : Controller
    {
        private readonly IAIService _aiService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ISituationService _situationService;    
        public SpekingAiSituationController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IAIService aiService, ISituationService situationService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _aiService = aiService;
            _situationService = situationService;
        }

        [HttpGet("Mentee/Role-Play/{situationId}/practice")]
        public async Task<IActionResult> Index(int situationId)
        {
            var situation = await _situationService.GetSituationByIdAsync(situationId);
            if (situation == null)
            {
                return NotFound();
            }

            // Truyền situationId và thông tin tình huống vào ViewBag
            ViewBag.SituationId = situationId;
            ViewBag.SituationTitle = situation.SituationName;
            ViewBag.SituationDescription = situation.Description;

            // Gọi logic SendMessage để AI nói trước
            string roleAi = situation.RoleAi;
            string roleUser = situation.RoleUser;
            string level = situation.Level.LevelName;
            string situationContext = $"Situation: {situation.SituationName}\nDescription: {situation.Description}\nYou are {roleAi}. The user is {roleUser}. Respond in English at a {level} level (e.g., use simple words and sentences for Beginner, more complex language for Advanced). Maintain your role as {roleAi} throughout the conversation and do not switch roles.";
            string initialPrompt = $"Start the conversation naturally as {roleAi}, greeting the user and offering assistance. Do not summarize the situation, just respond as {roleAi} would.";
            string aiReply = await _aiService.GetChatResponseAsync(initialPrompt, situationContext);
            string audioUrl = await GenerateSpeechAsync(aiReply);

            // Truyền câu trả lời mở đầu của AI vào ViewBag
            ViewBag.InitialAiMessage = aiReply;
            ViewBag.InitialAiAudioUrl = audioUrl;

            return View();
        }

       
      
        [HttpGet("Mentee/Role-Play")]
        public async Task<IActionResult> GetListSituationsAsync()
        {

            var listSituations = await _situationService.GetListSituationByRolePlay(Constants.ROLE_PLAY);
            return View("ListSituations", listSituations);
        }

        [HttpGet("Mentee/Role-Play/{situationId}")]
        public async Task<IActionResult> GetSituationByIdAsync(int situationId)
        {

            var situation = await _situationService.GetSituationByIdAsync(situationId);
            if (situation == null)
            {
                return NotFound();
            }
            return View("SituationDetail", situation);
        }





        // Xử lý tin nhắn text
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
                // KIỂM TRA: Thêm kiểm tra đầu vào để đảm bảo không rỗng
                if (request == null || string.IsNullOrEmpty(request.Message) || request.SituationId == 0)
                {
                    return Json(new { reply = "Please provide a message and select a situation." });
                }

                // Lấy thông tin tình huống từ database
                var situation = await _situationService.GetSituationByIdAsync(request.SituationId);
                if (situation == null)
                {
                    return Json(new { reply = "Situation not found." });
                }

                // Gửi thông tin tình huống (mô tả) vào AIService
                string roleAi = situation.RoleAi;
                string roleUser = situation.RoleUser;
                string level = situation.Level.LevelName;
                string situationContext = $"Situation: {situation.SituationName}\nDescription: {situation.Description}\nYou are {roleAi}. The user is {roleUser}. Respond in English at a {level} level (e.g., use simple words and sentences for Beginner, more complex language for Advanced). Maintain your role as {roleAi} throughout the conversation and do not switch roles.";
                string userMessage = request.Message;
              

                var reply = await _aiService.GetChatResponseAsync(userMessage, situationContext);
                if (string.IsNullOrEmpty(reply))
                {
                    return Json(new { reply = "AI could not generate a response. Please try again." });
                }

                var audioUrl = await GenerateSpeechAsync(reply);
                return Json(new { reply, audioUrl });
            }
            catch (Exception ex)
            {
                // XỬ LÝ LỖI: Trả về JSON với thông báo lỗi nếu có vấn đề
                return StatusCode(500, new { reply = "An error occurred while processing your request.", error = ex.Message });
            }
        }



        // THÊM: Phương thức để chỉ xử lý Speech-to-Text (Whisper API), trả về userMessage
        [HttpPost]
        public async Task<IActionResult> TranscribeAudio(IFormFile audio)
        {
            try
            {
                // KIỂM TRA: Đảm bảo file âm thanh không rỗng
                if (audio == null)
                {
                    return Json(new { userMessage = "", error = "No audio file provided." });
                }

                using var memoryStream = new MemoryStream();
                await audio.CopyToAsync(memoryStream);
                var audioBytes = memoryStream.ToArray();

                var userMessage = await _aiService.TranscribeAudioAsync(audioBytes);
                if (string.IsNullOrEmpty(userMessage))
                {
                    return Json(new { userMessage, error = "Could not transcribe audio. Please try again." });
                }

                return Json(new { userMessage });
            }
            catch (Exception ex)
            {
                // XỬ LÝ LỖI: Trả về JSON với thông báo lỗi nếu có vấn đề
                return StatusCode(500, new { userMessage = "", error = "An error occurred while processing audio.", details = ex.Message });
            }
        }



        //// Xử lý file âm thanh (Speech-to-Text)
        //[HttpPost]
        //public async Task<IActionResult> SendAudio(IFormFile audio, string situation)
        //{
        //    if (audio == null || string.IsNullOrEmpty(situation))
        //        return Json(new { userMessage = "", reply = "Please select a situation and provide audio." });

        //    // Chuyển file âm thanh thành byte[]
        //    using var memoryStream = new MemoryStream();
        //    await audio.CopyToAsync(memoryStream);
        //    var audioBytes = memoryStream.ToArray();

        //    // Gọi Whisper API qua ChatBotService
        //    var userMessage = await _aiService.TranscribeAudioAsync(audioBytes);
        //    if (string.IsNullOrEmpty(userMessage))
        //        return Json(new { userMessage, reply = "Could not transcribe audio. Please try again." });

        //    // Gọi GPT-3.5 qua ChatBotService
        //    var reply = await _aiService.GetChatResponseAsync(userMessage, situation);
        //    var audioUrl = await GenerateSpeechAsync(reply); // Azure TTS (tùy chọn)

        //    return Json(new { userMessage, reply, audioUrl });
        //}

        // Gọi Azure Text-to-Speech (tùy chọn)
        private async Task<string> GenerateSpeechAsync(string text)
        {
            var azureKey = _configuration["AzureSpeechKey"];
            var region = _configuration["AzureSpeechRegion"];
            if (string.IsNullOrEmpty(azureKey) || string.IsNullOrEmpty(region))
            {
                return null; // Bỏ qua nếu không dùng Azure
            }

            try
            {
                var ttsUrl = $"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1";
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureKey);
                client.DefaultRequestHeaders.Add("Content-Type", "application/ssml+xml");
                client.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", "audio-16khz-128kbitrate-mono-mp3");

                var ssml = $@"<speak version='1.0' xml:lang='en-US'>
                              <voice name='en-US-JennyNeural'>{text}</voice>
                           </speak>";
                var content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

                var response = await client.PostAsync(ttsUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var audioBytes = await response.Content.ReadAsByteArrayAsync();
                    var audioBase64 = Convert.ToBase64String(audioBytes);
                    return $"data:audio/mp3;base64,{audioBase64}";
                }

                return null;
            }
            catch (Exception ex)
            {
                // XỬ LÝ LỖI: Trả về null nếu Azure TTS thất bại
                return null;
            }
        }
    }

        public class ChatRequest
        {
            public string Message { get; set; }
        public int SituationId { get; set; }
    }

        public class WhisperResponse
        {
            public string text { get; set; }
        }

        public class OpenAIResponse
        {
            public Choice[] choices { get; set; }
            public class Choice
            {
                public Message message { get; set; }
            }
            public class Message
            {
                public string content { get; set; }
            }

        }
}
