using EXE_FAIEnglishTutor.Services.Interface.AI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using static EXE_FAIEnglishTutor.Areas.Mentee.Controllers.SpekingAiSituationController;

namespace EXE_FAIEnglishTutor.Services.Implimentaion.AI
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OpenAIService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["OpenAI:ApiKey"]}");
        }

        public async Task<string> CallOpenAIAsync(object requestBody)
        {
            try
            {
                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_configuration["OpenAI:Endpoint"], content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);

                return result.choices[0].message.content.Trim();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error calling OpenAI API: {ex.Message}");
            }
        }

        /// <summary>
        /// Gọi Gemini API với danh sách messages (hỗ trợ role: system, user, assistant).
        /// - role "system"    → system_instruction (Gemini xử lý riêng)
        /// - role "user"      → role "user" trong contents
        /// - role "assistant" → role "model" trong contents (Gemini dùng "model" thay vì "assistant")
        /// </summary>
        public async Task<string> CallGeminiAsync(List<Dictionary<string, string>> messages)
        {
            try
            {
                var apiKey = _configuration["Gemini:ApiKey"];
                var baseUrl = _configuration["Gemini:BaseUrl"];
                var model   = _configuration["Gemini:Model"];
                // URL format: {BaseUrl}/{Model}:generateContent?key={ApiKey}
                var url = $"{baseUrl}/{model}:generateContent?key={apiKey}";

                // Tách system message ra khỏi danh sách
                var systemMessage = messages.FirstOrDefault(m =>
                    m.GetValueOrDefault("role", "").Equals("system", StringComparison.OrdinalIgnoreCase));

                // Các message còn lại (user / assistant) → đưa vào contents
                var conversationMessages = messages
                    .Where(m => !m.GetValueOrDefault("role", "").Equals("system", StringComparison.OrdinalIgnoreCase))
                    .Select(m => new
                    {
                        // Gemini dùng "model" thay vì "assistant"
                        role = m.GetValueOrDefault("role", "user").Equals("assistant", StringComparison.OrdinalIgnoreCase)
                            ? "model"
                            : "user",
                        parts = new[] { new { text = m.GetValueOrDefault("content", "") } }
                    })
                    .ToList();

                // Build request body
                object requestBody;
                if (systemMessage != null)
                {
                    requestBody = new
                    {
                        system_instruction = new
                        {
                            parts = new[] { new { text = systemMessage.GetValueOrDefault("content", "") } }
                        },
                        contents = conversationMessages
                    };
                }
                else
                {
                    requestBody = new { contents = conversationMessages };
                }

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var geminiClient = new HttpClient();
                var response = await geminiClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Gemini API error {(int)response.StatusCode}: {errorBody}\nURL: {url.Replace(apiKey, "***")}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GeminiResponse>(responseJson);

                return result.candidates[0].content.parts[0].text.Trim();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error calling Gemini API: {ex.Message}");
            }
        }



        // Xử lý Speech-to-Text với Whisper API
        public async Task<string> TranscribeAudioAsync(byte[] audioBytes)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new ByteArrayContent(audioBytes), "file", "recording.mp3");
            formData.Add(new StringContent("whisper-1"), "model");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/audio/transcriptions", formData);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<WhisperResponse>(responseJson);
            return result.text?.Trim();
        }
    }

    public class OpenAIResponse
    {
        public Choice[] choices { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
    }
    public class WhisperResponse
    {
        public string text { get; set; }
    }

    // Gemini response models
    public class GeminiResponse
    {
        public GeminiCandidate[] candidates { get; set; }
    }

    public class GeminiCandidate
    {
        public GeminiContent content { get; set; }
    }

    public class GeminiContent
    {
        public GeminiPart[] parts { get; set; }
    }

    public class GeminiPart
    {
        public string text { get; set; }
    }
}

