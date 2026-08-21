using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using EXE_FAIEnglishTutor.Services.Interface.AI;

namespace EXE_FAIEnglishTutor.Services.Implimentaion.AI
{
    public class GeminiService : IAIService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public GeminiService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> GenerateTextAsync(string prompt)
        {
            var messages = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string> { { "role", "user" }, { "content", prompt } }
            };
            return await GenerateChatResponseAsync(messages);
        }

        public async Task<string> GenerateChatResponseAsync(List<Dictionary<string, string>> messages)
        {
            try
            {
                var apiKey = _configuration["Gemini:ApiKey"];
                var baseUrl = _configuration["Gemini:BaseUrl"];
                var model = _configuration["Gemini:Model"];
                var url = $"{baseUrl}/{model}:generateContent?key={apiKey}";

                var systemMessage = messages.FirstOrDefault(m =>
                    m.GetValueOrDefault("role", "").Equals("system", StringComparison.OrdinalIgnoreCase));

                var conversationMessages = messages
                    .Where(m => !m.GetValueOrDefault("role", "").Equals("system", StringComparison.OrdinalIgnoreCase))
                    .Select(m => {
                        var partsList = new List<object>();
                        if (m.ContainsKey("content") && !string.IsNullOrEmpty(m["content"]))
                        {
                            partsList.Add(new { text = m["content"] });
                        }

                        if (m.ContainsKey("imageBase64") && !string.IsNullOrEmpty(m["imageBase64"]))
                        {
                            var base64Str = m["imageBase64"];
                            var mimeType = "image/jpeg";
                            if (base64Str.StartsWith("data:"))
                            {
                                var partsSplit = base64Str.Split(';');
                                mimeType = partsSplit[0].Substring(5);
                                base64Str = partsSplit[1].Substring(7);
                            }
                            partsList.Add(new { inlineData = new { mimeType = mimeType, data = base64Str } });
                        }

                        if (!partsList.Any())
                        {
                            partsList.Add(new { text = "" });
                        }

                        return new
                        {
                            role = m.GetValueOrDefault("role", "user").Equals("assistant", StringComparison.OrdinalIgnoreCase)
                                ? "model"
                                : "user",
                            parts = partsList
                        };
                    })
                    .ToList();

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

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Gemini API error {(int)response.StatusCode}: {errorBody}\nURL: {url.Replace(apiKey, "***")}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GeminiResponse>(responseJson);

                return result.candidates[0].content.parts[0].text.Trim();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calling Gemini API: {ex.Message}");
            }
        }

        public async Task<string> TranscribeAudioAsync(byte[] audioBytes)
        {
            try
            {
                var apiKey = _configuration["Gemini:ApiKey"];
                var baseUrl = _configuration["Gemini:BaseUrl"];
                var model = _configuration["Gemini:Model"];
                var url = $"{baseUrl}/{model}:generateContent?key={apiKey}";

                var base64Audio = Convert.ToBase64String(audioBytes);

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new object[]
                            {
                                new { text = "Please transcribe the following audio accurately without adding any extra commentary." },
                                new { inlineData = new { mimeType = "audio/webm", data = base64Audio } }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Gemini Audio Transcription API error: {errorBody}");
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<GeminiResponse>(responseJson);

                return result.candidates[0].content.parts[0].text.Trim();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error transcribing audio with Gemini: {ex.Message}");
            }
        }
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

