using Microsoft.AspNetCore.Mvc;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using EXE_FAIEnglishTutor.Models;
using Microsoft.EntityFrameworkCore;

namespace EXE_FAIEnglishTutor.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    public class PodcastController : Controller
    {
        private readonly IPodcastService _podcastService;
        private const int PageSize = 6;

        public PodcastController(IPodcastService podcastService)
        {
            _podcastService = podcastService;
        }

        public async Task<IActionResult> Index(int page = 1, string topic = null)
        {
            try
            {
                var podcasts = await _podcastService.GetPostCard() ?? new List<Podcast>();
                ViewBag.Topics = podcasts.Select(p => p.Topic).Distinct().ToList();

                // Lọc theo chủ đề nếu có
                if (!string.IsNullOrEmpty(topic))
                {
                    podcasts = podcasts.Where(p => p.Topic == topic).ToList();
                }

                // Sắp xếp theo ngày giảm dần và tách podcast
                var orderedPodcasts = podcasts.OrderByDescending(p => p.CreatedAt).ToList();
                var newestPodcasts = orderedPodcasts.Take(2).ToList();
                var olderPodcasts = orderedPodcasts.Skip(2).ToList();

                // Phân trang cho các podcast cũ hơn
                var totalItems = olderPodcasts.Count;
                var totalPages = (int)Math.Ceiling(totalItems / 3.0); // PageSize = 3
                page = Math.Max(1, totalPages == 0 ? 1 : Math.Min(page, totalPages));
                var paginatedOlderPodcasts = olderPodcasts
                    .Skip((page - 1) * 3)
                    .Take(3)
                    .ToList();

                // Truyền dữ liệu vào view
                ViewBag.NewestPodcasts = newestPodcasts;
                ViewBag.PaginatedOlderPodcasts = paginatedOlderPodcasts;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentTopic = topic;

                return View("ListPodcasts", podcasts);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần
                return View("Error");
            }
        }

        public async Task<IActionResult> ListenPC(int id)
        {
            var podcasts = await _podcastService.GetPostCard();
            var podcast = podcasts.FirstOrDefault(p => p.Id == id);
            
            if (podcast == null)
            {
                return NotFound();
            }

            return View(podcast);
        }
    }
}
