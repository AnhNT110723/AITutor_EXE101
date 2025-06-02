using EXE_FAIEnglishTutor.Common;
using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Implimentaion.AI;
using EXE_FAIEnglishTutor.Services.Interface;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class SpekingAiSituationController : Controller
    {
        private readonly IOpenAIService _aiService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ISituationService _situationService; 
        private readonly IUserService _userService; 
        private readonly ISpeakingAIService _speakingAiService; 
        
        public SpekingAiSituationController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IOpenAIService aiService, ISituationService situationService, IUserService userService, ISpeakingAIService speakingAIService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _aiService = aiService;
            _situationService = situationService;
            _userService = userService;
            _speakingAiService = speakingAIService;
        }

        [HttpGet("Mentee/Role-Play/{situationId}/practice")]
        public async Task<IActionResult> Index(int situationId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException();
            }
            var situation = await _situationService.GetSituationByIdAsync(situationId);
            if (situation == null)
            {
                return NotFound();
            }

            // Truyền situationId và thông tin tình huống vào ViewBag
            ViewBag.SituationId = situationId;
            ViewBag.SituationTitle = situation.SituationName;
            ViewBag.SituationDescription = situation.Description;
            int userId = int.Parse(userIdClaim.Value);
            var user = await _userService.GetUserById(userId);
            if (user == null) { return NotFound(); }
            ViewBag.UserId = user.UserId;
            ViewBag.SituationId = situationId;
            ViewBag.TypeId = situation.TypeId;

            // Gọi logic SendMessage để AI nói trước
            //string roleAi = situation.RoleAi;
            //string roleUser = situation.RoleUser;
            //string level = situation.Level.LevelName;
            //string situationContext = $"Situation: {situation.SituationName}\nDescription: {situation.Description}\nYou are {roleAi}. The user is {roleUser}. Respond in English at a {level} level (e.g., use simple words and sentences for Beginner, more complex language for Advanced). Maintain your role as {roleAi} throughout the conversation and do not switch roles.";
            //string initialPrompt = $"Start the conversation naturally as {roleAi}, greeting the user and offering assistance. Do not summarize the situation, just respond as {roleAi} would.";
            //string aiReply = await _speakingAiService.GetChatResponseAsync(initialPrompt, situationContext);
            //string audioUrl = await GenerateSpeechAsync(aiReply);
            //// Truyền câu trả lời mở đầu của AI vào ViewBag 
            //ViewBag.InitialAiMessage = aiReply;
            //ViewBag.InitialAiAudioUrl = audioUrl;

            return View();
        }


        [HttpPost("Mentee/SpekingAiSituation/StartConversation")]
        public async Task<IActionResult> StartConversation([FromBody] StartConversationRequest request)
        {
            try
            {
                if (request == null || request.SituationId <= 0)
                {
                    return BadRequest(new { error = "Invalid situation ID." });
                }

                var situation = await _situationService.GetSituationByIdAsync(request.SituationId);
                if (situation == null)
                {
                    return Json(new { reply = "Situation not found." });
                }

                string roleAi = situation.RoleAi;
                string roleUser = situation.RoleUser;
                string level = situation.Level.LevelName;
                string situationContext = $"Situation: \"{situation.SituationName}\"\nDescription: {situation.Description}\r\nYou are a {roleAi}. The user is a {roleUser}. Respond in English at a {level} level (e.g., use simple words and sentences for Beginner, more complex language for Advanced). Maintain your role as a {roleAi} throughout the conversation and do not switch roles.";
                string initialPrompt = $"Start the conversation naturally as {roleAi}, greeting the user and offering assistance. Do not summarize the situation, just respond as {roleAi} would.";

                string aiReply = await _speakingAiService.GetChatResponseAsync(initialPrompt, situationContext);
                if (string.IsNullOrEmpty(aiReply))
                {
                    return Json(new { reply = "Error: AI could not generate a response." });
                }

                string voice = string.IsNullOrEmpty(request.Voice) ? "en-US-JennyNeural" : request.Voice;
                string audioUrl = await GenerateSpeechAsync(aiReply, voice);
                if (audioUrl == null)
                {
                    Console.WriteLine("Failed to generate audio for initial message.");
                }

                return Json(new { reply = aiReply, audioUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in StartConversation: {ex.Message}");
                return StatusCode(500, new { error = "Error starting conversation.", details = ex.Message });
            }
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException();
            }
            var situation = await _situationService.GetSituationByIdAsync(situationId);
            if (situation == null)
            {
                return NotFound();
            }
            int userId = int.Parse(userIdClaim.Value);
            var user = await _userService.GetUserById(userId);
            if (user == null) { return NotFound(); }
            ViewBag.UserId = user.UserId;
            ViewBag.SituationId = situationId;
            ViewBag.TypeId = situation.TypeId;
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
              

                var reply = await _speakingAiService.GetChatResponseAsync(userMessage, situationContext);
                if (string.IsNullOrEmpty(reply))
                {
                    return Json(new { reply = "AI could not generate a response. Please try again." });
                }

                var audioUrl = await GenerateSpeechAsync(reply, request.Voice ?? "en-US-JennyNeural");
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


        [HttpPost("Mentee/SpekingAiSituation/GenerateAudio")]
        public async Task<IActionResult> GenerateAudio([FromBody] GenerateAudioRequest request)
        {
            if (string.IsNullOrEmpty(request.Text))
            {
                return BadRequest("Text is required.");
            }

            try
            {
                var audioUrl = await GenerateSpeechAsync(request.Text, request.Voice);
                if (audioUrl == null)
                {
                    return StatusCode(500, "Failed to generate audio.");
                }
                return Ok(new { audioUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating audio: {ex.Message}");
                return StatusCode(500, "Error generating audio.");
            }
        }




        // Gọi Azure Text-to-Speech (tùy chọn)
        private async Task<string> GenerateSpeechAsync(string text, string voiceName)
        {
            var azureKey = _configuration["Azure:Speech:SubscriptionKey"];
            var region = _configuration["Azure:Speech:Region"];
            if (string.IsNullOrEmpty(azureKey) || string.IsNullOrEmpty(region))
            {
                return null; // Bỏ qua nếu không dùng Azure
            }

            try
            {
                var ttsUrl = $"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1";
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureKey);
                client.DefaultRequestHeaders.Add("X-Microsoft-OutputFormat", "audio-16khz-128kbitrate-mono-mp3");
                client.DefaultRequestHeaders.Add("User-Agent", "AiTutor/1.0 (AiTutor@example.com)");

                var ssml = $@"<speak version='1.0' xml:lang='en-US'>
                              <voice name='{voiceName}'>{text}</voice>
                           </speak>";
                var content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

                var response = await client.PostAsync(ttsUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var audioBytes = await response.Content.ReadAsByteArrayAsync();
                    var audioBase64 = Convert.ToBase64String(audioBytes);
                    Console.WriteLine($"Base64 Length: ${audioBase64.Length}, Preview: {audioBase64.Substring(0, Math.Min(50, audioBase64.Length))}");
                    return $"data:audio/mp3;base64,{audioBase64}";
                }

                return null;
            }
            catch (Exception ex)
            {
                // XỬ LÝ LỖI: Trả về null nếu Azure TTS thất bại
                Console.WriteLine($"Lỗi Azure TTS: {ex.Message}");
                return null;
            }
        }
    }

    public class GenerateAudioRequest
    {
        public string Text { get; set; }
        public string Voice { get; set; }
    }
    public class ChatRequest
        {
            public string Message { get; set; }
            public int SituationId { get; set; }
            public string Voice { get; set; }
    }

    public class StartConversationRequest
    {
        public int SituationId { get; set; }
        public string Voice { get; set; }
    }

}
