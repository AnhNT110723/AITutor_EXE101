using EXE_FAIEnglishTutor.Services.Interface.AI;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

namespace EXE_FAIEnglishTutor.Services.Implimentaion.AI
{
    public class OpenAIService : IAIService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OpenAIService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuration["OpenAI:ApiKey"]}");
        }

        public async Task<string> GenerateTextAsync(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[] { new { role = "user", content = prompt } },
                    max_tokens = 500
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_configuration["OpenAI:Endpoint"], content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);

                return result.choices[0].message.content.Trim();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calling OpenAI API (GenerateTextAsync): {ex.Message}");
            }
        }

        public async Task<string> GenerateChatResponseAsync(List<Dictionary<string, string>> messages)
        {
            try
            {
                // Chuyển đổi format của messages sang format của OpenAI
                var openAiMessages = messages.Select(m => new
                {
                    role = m.GetValueOrDefault("role", "user"),
                    content = m.GetValueOrDefault("content", "")
                }).ToList();

                var requestBody = new
                {
                    model = "gpt-3.5-turbo",
                    messages = openAiMessages
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_configuration["OpenAI:Endpoint"], content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OpenAIResponse>(responseJson);

                return result.choices[0].message.content.Trim();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calling OpenAI API (GenerateChatResponseAsync): {ex.Message}");
            }
        }

        public async Task<string> TranscribeAudioAsync(byte[] audioBytes)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error transcribing audio with OpenAI Whisper: {ex.Message}");
            }
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

