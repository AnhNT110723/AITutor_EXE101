using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Implementaion.AI;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using EXE_FAIEnglishTutor.Services.Interface;

namespace EXE_FAIEnglishTutor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostcardController : ControllerBase
    {
        private readonly SpeechService _speechService;
        private readonly IPodcastService _podcastService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IHttpClientFactory _httpClientFactory;

        public PostcardController(
            SpeechService speechService,
            IPodcastService podcastService,
            ICloudinaryService cloudinaryService,
            IHttpClientFactory httpClientFactory)
        {
            _speechService = speechService;
            _podcastService = podcastService;
            _cloudinaryService = cloudinaryService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("generate-podcast")]
        public async Task<IActionResult> GeneratePodcast([FromBody] PodcastRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request?.Topic) || string.IsNullOrWhiteSpace(request?.Title))
                    return BadRequest("Thiếu chủ đề hoặc tiêu đề podcast.");

                string promptText = $"Viết một đoạn podcast tiếng Anh khoảng 300-500 từ về: {request.Topic}";

                // 1. Gen text content
                var content = await _speechService.GeneratePostcardAsync(promptText);

                if (string.IsNullOrWhiteSpace(content))
                    return BadRequest($"Không nhận được nội dung cho podcast {request.Title}.");

                // 2. Gọi API tạo audio từ text
                var audioBytes = await GenerateAudioFromText(content);
                if (audioBytes == null)
                    return StatusCode(500, $"Không tạo được audio cho podcast {request.Title}.");

                // 3. Upload audio lên Cloudinary
                var audioUrl = await _cloudinaryService.UploadAudioAsync(audioBytes, $"podcast_{DateTime.UtcNow.Ticks}");
                if (string.IsNullOrEmpty(audioUrl))
                    return StatusCode(500, $"Không up được audio lên Cloudinary cho podcast {request.Title}.");

                var podcast = new Podcast
                {
                    Title = request.Title,
                    Content = content,
                    ImageUrl = "/Images/a.jpg", // hoặc tạo ảnh ngẫu nhiên nếu cần
                    CreatedAt = DateTime.UtcNow,
                    UserId = null, // Nếu có user đăng nhập thì lấy id user
                    Topic = request.Topic,
                    AudioUrl = audioUrl
                };

                await _podcastService.SavePodcastsAsync(new List<Podcast> { podcast });

                return Ok(new
                {
                    message = "Đã tạo podcast thành công.",
                    podcast
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi khi tạo podcast.", detail = ex.Message });
            }
        }

        // Hàm helper gọi API AudioController
        private async Task<byte[]?> GenerateAudioFromText(string text)
        {
            var client = _httpClientFactory.CreateClient();
            var url = "http://localhost:5037/api/audio/generate-english"; // Đổi lại port nếu khác

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(text), "text");

            var response = await client.PostAsync(url, form);
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsByteArrayAsync();
        }
    }

    public class PodcastRequestDto
    {
        public string Topic { get; set; }
        public string Title { get; set; }
    }
}