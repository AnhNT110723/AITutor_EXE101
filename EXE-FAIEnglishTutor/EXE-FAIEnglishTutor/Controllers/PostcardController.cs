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

        [HttpGet("get-all-podcasts")]
        public async Task<IActionResult> GetAllPodcasts()
        {
            try
            {
                var podcasts = await _podcastService.GetPostCard();
                return Ok(podcasts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi khi lấy danh sách podcasts.", detail = ex.Message });
            }
        }

        [HttpPost("generate-podcasts")]
        public async Task<IActionResult> GeneratePodcasts()
        {
            try
            {
                var prompts = new List<string>
            {
                "Tóm tắt tin tức mới nhất ở Việt Nam",
                "Tóm tắt tin tức mới nhất trên thế giới",
                "Tóm tắt tin tức kinh tế gần đây",
                "Tóm tắt tin tức giáo dục gần đây",
                "Tóm tắt tin tức thể thao gần đây",
                "Tạo một đoạn hội thoại về chủ đề ngẫu nhiên",
                "Tạo một đoạn hội thoại về chủ đề ngẫu nhiên",
                "Tạo một đoạn hội thoại về chủ đề ngẫu nhiên",
                "Tạo một đoạn hội thoại về chủ đề ngẫu nhiên",
                "Tạo một đoạn hội thoại về chủ đề ngẫu nhiên"
            };

                var titles = new List<string>
            {
                "Tin tức mới nhất Việt Nam",
                "Tin tức thế giới",
                "Tin tức kinh tế",
                "Tin tức giáo dục",
                "Tin tức thể thao",
                "Đoạn hội thoại 1",
                "Đoạn hội thoại 2",
                "Đoạn hội thoại 3",
                "Đoạn hội thoại 4",
                "Đoạn hội thoại 5"
            };

                var podcasts = new List<Podcast>();

                for (int i = 0; i < prompts.Count; i++)
                {
                    var content = await _speechService.GeneratePostcardAsync("Tạo một đoạn podcast bằng tiếng anh khoảng 10000 từ về:"+prompts[i]);
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        return BadRequest($"Không nhận được nội dung cho podcast {titles[i]}.");
                    }

                    var podcast = new Podcast
                    {
                        Title = titles[i],
                        Content = content,
                        ImageUrl = "/Images/a.jpg",
                        CreatedAt = DateTime.UtcNow
                    };

                    podcasts.Add(podcast);
                }

                await _podcastService.SavePodcastsAsync(podcasts);

                return Ok(new { message = "Đã tạo 10 podcast thành công.", podcasts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Lỗi khi tạo podcasts.", detail = ex.Message });
            }
        }
    }
}
