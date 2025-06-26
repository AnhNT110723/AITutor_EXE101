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
                var podcasts = await _podcastService.GetPostCard();
                // Loại bỏ ký tự đặc biệt khỏi Content
                foreach (var p in podcasts)
                {
                    p.Content = FilterSpecialCharacters(p.Content);
                }
                ViewBag.Topics = podcasts.Select(p => p.Topic).Distinct().ToList();

                // Lọc theo chủ đề nếu có
                if (!string.IsNullOrEmpty(topic))
                {
                    podcasts = podcasts.Where(p => p.Topic.Equals(topic)).ToList();
                }
                if (podcasts.Count() > 5)
                {

                    // Sắp xếp theo ngày giảm dần và tách podcast
                    var orderedPodcasts = podcasts.OrderByDescending(p => p.CreatedAt).ToList();
                    var newestPodcasts = orderedPodcasts.Take(2).ToList();
                    var olderPodcasts = orderedPodcasts.Skip(2).ToList();

                    // Phân trang cho các podcast cũ hơn
                    var totalItems = olderPodcasts.Count;
                    var totalPages = (int)Math.Ceiling(totalItems / 5.0);
                    page = Math.Max(1, totalPages == 0 ? 1 : Math.Min(page, totalPages));
                    var paginatedOlderPodcasts = olderPodcasts
                        .Skip((page - 1) * 5)
                        .Take(5)
                        .ToList();

                    // Truyền dữ liệu vào view
                    ViewBag.NewestPodcasts = newestPodcasts;
                    ViewBag.PaginatedOlderPodcasts = paginatedOlderPodcasts;
                    ViewBag.CurrentPage = page;
                    ViewBag.TotalPages = totalPages;
                    ViewBag.CurrentTopic = topic;

                }
                else
                {
                    ViewBag.NewestPodcasts = podcasts;
                    ViewBag.PaginatedOlderPodcasts = podcasts;
                    ViewBag.CurrentPage = 1;
                    ViewBag.TotalPages = 1;
                    ViewBag.CurrentTopic = topic;
                }



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
            // Kiểm tra người dùng đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                // Nếu chưa đăng nhập, chuyển hướng sang trang Login
                return RedirectToAction("Login", "Account");
            }

            var podcasts = await _podcastService.GetPostCard();
            var podcast = podcasts.FirstOrDefault(p => p.Id == id);

            if (podcast == null)
            {
                return NotFound();
            }

            // Lọc ký tự đặc biệt ở các trường cần thiết, ví dụ Title và Description
            podcast.Title = FilterSpecialCharacters(podcast.Title);
            podcast.Content = FilterSpecialCharacters(podcast.Content);

            return View(podcast);
        }

        // Hàm hỗ trợ lọc ký tự đặc biệt và entity HTML
        private string FilterSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // 1. Loại bỏ entity HTML &...; hoặc &#...;
            string noEntities = System.Text.RegularExpressions.Regex.Replace(input, "&[#a-zA-Z0-9]+;", "");

            // 2. Loại bỏ xuống dòng cứng (nếu muốn giữ lại thì comment dòng này)
            noEntities = noEntities.Replace("\r", "").Replace("\n", "");

            // 3. Loại bỏ mọi ký tự không mong muốn, chỉ giữ lại chữ, số, dấu cách, ., ,, ?, !, -
            var sb = new System.Text.StringBuilder();
            foreach (char c in noEntities)
            {
                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '.' || c == ',' || c == '?' || c == '!' || c == '-')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
