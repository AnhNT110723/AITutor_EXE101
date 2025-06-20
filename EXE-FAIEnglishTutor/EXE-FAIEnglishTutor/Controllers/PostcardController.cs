using EXE_FAIEnglishTutor.Dtos;
using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Implementaion.AI;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EXE_FAIEnglishTutor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostcardController : ControllerBase
    {
        private readonly SpeechService _speechService;
        private readonly IPodcastService _podcastService;
        public PostcardController(SpeechService speechService,IPodcastService podcastService)
        {   
            _speechService = speechService;
            _podcastService = podcastService;
        }

        //[HttpGet("get-all-podcasts")]
        //public async Task<IActionResult> GetAllPodcasts()
        //{
        //    try
        //    {
        //        var podcasts = await _podcastService.GetPostCard();
        //        return Ok(podcasts);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = "Lỗi khi lấy danh sách podcasts.", detail = ex.Message });
        //    }
        //}

        [HttpPost("generate-podcasts")]
        public async Task<IActionResult> GeneratePodcasts()
        {
            try
            {
                var prompts = new List<(string Prompt, string Title, string Topic)>
        {
            ("Tóm tắt tin tức mới nhất ở Việt Nam", "Tin tức mới nhất Việt Nam", "News"),
            ("Tóm tắt tin tức mới nhất trên thế giới", "Tin tức thế giới", "News"),
            ("Tóm tắt tin tức kinh tế gần đây", "Tin tức kinh tế", "Economy"),
            ("Tóm tắt tin tức giáo dục gần đây", "Tin tức giáo dục", "Education"),
            ("Tóm tắt tin tức thể thao gần đây", "Tin tức thể thao", "Sports"),
            ("Tạo một đoạn hội thoại về chủ đề ngẫu nhiên", "Đoạn hội thoại 1", "Conversation"),
            ("Tạo một đoạn hội thoại về chủ đề ngẫu nhiên", "Đoạn hội thoại 2", "Conversation"),
            ("Tạo một đoạn hội thoại về chủ đề ngẫu nhiên", "Đoạn hội thoại 3", "Conversation"),
            ("Tạo một đoạn hội thoại về chủ đề ngẫu nhiên", "Đoạn hội thoại 4", "Conversation"),
            ("Tạo một đoạn hội thoại về chủ đề ngẫu nhiên", "Đoạn hội thoại 5", "Conversation"),
        };

                var podcasts = new List<Podcast>();

                for (int i = 0; i < prompts.Count; i++)
                {
                    string promptText = $"Viết một đoạn podcast tiếng Anh khoảng 300-500 từ về: {prompts[i].Prompt}";

                    var content = await _speechService.GeneratePostcardAsync(promptText);

                    if (string.IsNullOrWhiteSpace(content))
                    {
                        return BadRequest($"Không nhận được nội dung cho podcast {prompts[i].Title}.");
                    }

                    var podcast = new Podcast
                    {
                        Title = prompts[i].Title,
                        Content = content,
                        ImageUrl = "/Images/a.jpg", // hoặc tạo ảnh ngẫu nhiên nếu cần
                        CreatedAt = DateTime.UtcNow,
                        UserId = null, // bạn có thể gán ID user cụ thể nếu có đăng nhập
                        Topic = prompts[i].Topic
                    };

                    podcasts.Add(podcast);
                }

                await _podcastService.SavePodcastsAsync(podcasts);

                return Ok(new
                {
                    message = "Đã tạo 10 podcast thành công.",
                    podcasts
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Lỗi khi tạo podcasts.",
                    detail = ex.Message
                });
            }
        }


    }
}
