using EXE_FAIEnglishTutor.Services.Interface.AI;
using System.Text;
using System.Text.Json;
using static EXE_FAIEnglishTutor.Areas.Mentee.Controllers.SpekingAiSituationController;

namespace EXE_FAIEnglishTutor.Services.Implimentaion.AI
{
    public class AIService : IAIService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public AIService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["OpenAI:ApiKey"]}");
        }

        public async Task<string> GetChatResponseAsync(string userMessage, string situation = null)
        {
            var systemPrompt = situation != null
                ? $"You are an English tutor specializing in spoken English. Focus strictly on the following situation and do not deviate from the topic: {situation}. " +
                  $"Provide a concise, natural, and conversational response in English suitable for speaking practice. Start the conversation if the user requests it."
                : "You are a helpful assistant. Provide a concise and conversational response.";

            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userMessage }
                },
                max_tokens = 100
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_configuration["OpenAI:Endpoint"], content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);

            return result.choices[0].message.content.Trim();
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
}

