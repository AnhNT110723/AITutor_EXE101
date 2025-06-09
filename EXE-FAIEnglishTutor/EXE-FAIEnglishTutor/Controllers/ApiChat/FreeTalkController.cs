using EXE_FAIEnglishTutor.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace EXE_FAIEnglishTutor.Controllers.ApiChat
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreeTalkController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public FreeTalkController(IConfiguration config, HttpClient httpClient)
        {
            _config = config;
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            var endpoint = _config["OpenAI:Endpoint"];

            var systemMessage = "You are a friendly and knowledgeable English teacher. If the user's sentence contains grammar or vocabulary mistakes, provide a corrected version and briefly explain the reason. If the sentence is correct, respond in a natural, conversational way and ask follow-up questions to keep the conversation going.";

            var payload = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = systemMessage },
                    new { role = "user", content = request.Message }
                }
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Headers.Add("Authorization", $"Bearer {apiKey}");
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "OpenAI error");

            var responseContent = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(responseContent);
            var reply = json.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();

            // Nếu bạn muốn phân tích phần sửa lỗi: (ví dụ kiểm tra có từ "Corrected" hoặc "should say")
            string suggestion = null;
            if (reply.Contains("correct") || reply.Contains("should say") || reply.Contains("instead of"))
                suggestion = reply;

            return Ok(new ChatResponse
            {
                Reply = reply,
                Suggestion = suggestion
            });
        }


    }
}
