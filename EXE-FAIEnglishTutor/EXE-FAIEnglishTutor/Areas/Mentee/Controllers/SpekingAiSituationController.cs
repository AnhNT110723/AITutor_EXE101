using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Services.Implimentaion.AI;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class SpekingAiSituationController : Controller
    {
        private readonly IAIService _aiService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public SpekingAiSituationController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IAIService aiService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _aiService = aiService;
        }
        public IActionResult Index()
        {
            return View(new ChatViewModel());
        }
        public IActionResult Index1()
        {
            return View(new ChatViewModel());
        }

        // Xử lý tin nhắn text
        // Xử lý tin nhắn text
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
                // KIỂM TRA: Thêm kiểm tra đầu vào để đảm bảo không rỗng
                if (request == null || string.IsNullOrEmpty(request.Message) || string.IsNullOrEmpty(request.Situation))
                {
                    return Json(new { reply = "Please provide a message and select a situation." });
                }

                var reply = await _aiService.GetChatResponseAsync(request.Message, request.Situation);
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
            public string Situation { get; set; }
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
