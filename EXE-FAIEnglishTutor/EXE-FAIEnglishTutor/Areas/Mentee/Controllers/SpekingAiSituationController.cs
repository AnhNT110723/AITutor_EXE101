using EXE_FAIEnglishTutor.Common;
using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Implimentaion.AI;
using EXE_FAIEnglishTutor.Services.Interface;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
        private readonly AzureTranslatorConfig _azureTranslatorConfig;
        private readonly HttpClient _httpClient;
        public SpekingAiSituationController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IOpenAIService aiService, ISituationService situationService, IUserService userService, ISpeakingAIService speakingAIService, IOptions<AzureTranslatorConfig> azureTranslatorConfig
        )
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _aiService = aiService;
            _situationService = situationService;
            _userService = userService;
            _speakingAiService = speakingAIService;
            _azureTranslatorConfig = azureTranslatorConfig.Value;
            _httpClient = httpClientFactory.CreateClient();
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
                var messages = new List<object>
                {
                    new { role = "system", content = situationContext },
                    new { role = "user", content = initialPrompt }
                };
                string aiReply = await _speakingAiService.GetChatResponseAsync(messages);
                if (string.IsNullOrEmpty(aiReply))
                {
                    return Json(new { reply = "Error: AI could not generate a response." });
                }

                // Dịch AI reply sang tiếng Việt (dùng Azure Translator)
                string translatedReply = await TranslateTextAsync(aiReply, Constants.TARGET_LANG);
                
                string voice = string.IsNullOrEmpty(request.Voice) ? "en-US-JennyNeural" : request.Voice;
                string audioUrl = await GenerateSpeechAsync(aiReply, voice);
                if (audioUrl == null)
                {
                    Console.WriteLine("Failed to generate audio for initial message.");
                }

                return Json(new { reply = aiReply, translatedReply, audioUrl });
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                int userId = int.Parse(userIdClaim.Value);
                var user = await _userService.GetUserById(userId);
                if (user.ExpiryDate != null && user.UpgradeLevel > 0)
                {
                    ViewBag.userCheckMember = true;
                }
            }
            var levels = await _situationService.GetAllLevelAsync();
            var listSituations = await _situationService.GetListSituationByRolePlay(Constants.ROLE_PLAY);
            ViewBag.levels= levels;

            return View("ListSituations", listSituations);
        }
       
        [HttpGet("Mentee/Role-Play/ListPartial")]
        public async Task<IActionResult> GetListSituationsPartialAsync(string keyword = "", string category = "")
        {
            try
            {
              
                keyword = string.IsNullOrWhiteSpace(keyword) ? "" : keyword.Trim();
                category = string.IsNullOrWhiteSpace(category) ? "" : category.Trim();

                
                var listSituations = await _situationService.GetListSituationByRolePlay(Constants.ROLE_PLAY, keyword, category);

                // Chọn chỉ các thuộc tính cần thiết
                var result = listSituations.Select(s => new
                { 
                    situationId = s.SituatuonId,
                    situationName = s.SituationName,
                    imageUrl = s.ImageUrl,
                    level = new
                    {
                        levelName = s.Level?.LevelName ?? "Unknown"
                    }
                }).ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi nếu cần (tùy thuộc vào hệ thống logging của bạn)
                return StatusCode(500, new { error = "Đã xảy ra lỗi khi tải danh sách tình huống." });
            }
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
                if (request == null || request.Messages == null || !request.Messages.Any() || request.SituationId == 0)
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
                string situationContext = $"Situation: {situation.SituationName}\nDescription: {situation.Description}\nYou are {roleAi}. The user is {roleUser}. Respond in English at a {level} level (e.g., use simple words and sentences for Beginner, more complex language for Advanced). Maintain your role as {roleAi} throughout the conversation and do not switch roles. Continue the conversation naturally based on the provided chat history, avoiding repetitive greetings like 'Hello' or 'Hi' unless appropriate.";
                // Chuyển đổi danh sách tin nhắn từ client sang định dạng phù hợp cho OpenAI
                var messages = new List<object>
                {
                    new { role = "system", content = situationContext }
                };

                messages.AddRange(request.Messages.Select(m => new { role = m.Role, content = m.Content }));
                var reply = await _speakingAiService.GetChatResponseAsync(messages);
                if (string.IsNullOrEmpty(reply))
                {
                    return Json(new { reply = "AI could not generate a response. Please try again." });
                }
                // Dịch AI reply sang tiếng Việt
                string translatedReply = await TranslateTextAsync(reply, Constants.TARGET_LANG);
                var audioUrl = await GenerateSpeechAsync(reply, request.Voice ?? "en-US-JennyNeural");
                return Json(new { reply, translatedReply, audioUrl });
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


        [HttpPost("Mentee/SpekingAiSituation/GetSuggestion")]
        public async Task<IActionResult> GetSuggestion([FromBody] SuggestionRequest request)
        {
            try
            {
                if (request == null || request.SituationId <= 0)
                {
                    return BadRequest(new { error = "Invalid situation ID." });
                }

                string suggestion = await GenerateSuggestion(request.SituationId, request.Messages);
                // Dịch gợi ý sang tiếng Việt nếu cần
               // string translatedSuggestion = await TranslateTextAsync(suggestion, "vi");
                return Ok(new { suggestion });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSuggestion: {ex.Message}");
                return StatusCode(500, new { error = "Could not generate suggestion.", details = ex.Message });
            }
        }

        [HttpPost("Mentee/SpekingAiSituation/EvaluateMessage")]
        public async Task<IActionResult> EvaluateMessage([FromBody] EvaluateMessageRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Message))
                {
                    return BadRequest(new { error = "Message is required." });
                }

                // Tạo prompt để yêu cầu AI đánh giá ngữ pháp
                string prompt = $@"Please evaluate the following sentence for grammar and suggest a corrected version if needed. If the sentence is already correct, confirm it. Provide a concise explanation and a corrected sentence (if applicable) in the format:
                                Explanation: [Your explanation]
                                Corrected: [Corrected sentence or original if correct]
                                Sentence: {request.Message}";

                var messages = new List<object>
        {
            new { role = "user", content = prompt }
        };

                string suggestion = await _speakingAiService.GetChatResponseAsync(messages);
                if (string.IsNullOrEmpty(suggestion))
                {
                    return Json(new { suggestion = "Could not evaluate the message." });
                }

                return Json(new { suggestion });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EvaluateMessage: {ex.Message}");
                return StatusCode(500, new { error = "Error evaluating message.", details = ex.Message });
            }
        }
        private async Task<string> GenerateSuggestion(int situationId, List<Message> messages)
        {
            // Lấy thông tin tình huống
            var situation = await _situationService.GetSituationByIdAsync(situationId);
            if (situation == null)
            {
                throw new Exception("Situation not found.");
            }

            // Tạo ngữ cảnh cho AI
            string roleAi = situation.RoleAi;
            string roleUser = situation.RoleUser;
            string level = situation.Level.LevelName;
            string situationContext = $"Situation: \"{situation.SituationName}\"\nDescription: {situation.Description}\r\nYou are a {roleAi}. The user is a {roleUser}. Respond in English at a {level} level (e.g., use simple words and sentences for Beginner, more complex language for Advanced). Maintain your role as a {roleAi} throughout the conversation and do not switch roles.";

            // Tạo prompt để yêu cầu gợi ý
            string conversation = messages.Any()
                ? string.Join("\n", messages.Select(m => $"{m.Role}: {m.Content}"))
                : "No conversation yet.";
            string prompt = $@" Provide a concise suggestion (1 sentence) for what the user (as {roleUser}) could say next to continue the conversation naturally, based on the following context and conversation:
            Context: {situationContext} Conversation:{conversation}";

            // Chuẩn bị messages cho AI
            var aiMessages = new List<object>
            {
                new { role = "system", content = situationContext },
                new { role = "user", content = prompt }
            };

            // Gọi AI để tạo gợi ý
            string suggestion = await _speakingAiService.GetChatResponseAsync(aiMessages);
            if (string.IsNullOrEmpty(suggestion))
            {
                throw new Exception("AI could not generate a suggestion.");
            }

            return suggestion.Trim();
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


        // Phương thức dịch văn bản (Azure Translator)
        private async Task<string> TranslateTextAsync(string text, string targetLang)
        {
            try
            {
                Console.WriteLine($"Config: ApiKey={_azureTranslatorConfig.ApiKey?.Substring(0, 4)}..., Endpoint={_azureTranslatorConfig.Endpoint}, Region={_azureTranslatorConfig.Region}");
                Console.WriteLine($"Translating text: {text} to {targetLang}");
                string endpoint = $"{_azureTranslatorConfig.Endpoint}/translate?api-version=3.0";

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _azureTranslatorConfig.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Region", _azureTranslatorConfig.Region);

                var requestBody = new[]
                {
                    new { Text = text }
                };
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{endpoint}&to={targetLang}", content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Translation failed: {response.StatusCode}");
                    return text; // Trả về text gốc nếu dịch thất bại
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<TranslationResponse>>(responseBody, options);
                if (result == null || result.Count == 0 || result[0].Translations == null || result[0].Translations.Count == 0)
                {
                    Console.WriteLine("Invalid translation response format.");
                    return text;
                }

                var translatedText = result[0].Translations[0].Text;
                Console.WriteLine($"Translated text: {translatedText}");
                return translatedText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error translating text: {ex.Message}");
                return text; // Trả về text gốc nếu có lỗi
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
        public List<Message> Messages { get; set; }
        public int SituationId { get; set; }
        public string Voice { get; set; }
    }
    public class SuggestionRequest
    {
        public int SituationId { get; set; }
        public List<Message> Messages { get; set; }
    }

    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }
    public class StartConversationRequest
    {
        public int SituationId { get; set; }
        public string Voice { get; set; }
    }

    public class AzureTranslatorConfig
    {
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
        public string Region { get; set; }
    }

    public class TranslationResponse
    {
        public DetectedLanguage DetectedLanguage { get; set; }
        public List<Translation> Translations { get; set; }
    }

    public class DetectedLanguage
    {
        public string Language { get; set; }
        public float Score { get; set; }
    }

    public class Translation
    {
        public string Text { get; set; }
        public string To { get; set; }
    }

    public class EvaluateMessageRequest
{
    public string Message { get; set; }
}

}
