using EXE_FAIEnglishTutor.Models;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using Microsoft.EntityFrameworkCore;

namespace EXE_FAIEnglishTutor.Services.Implementaion.AI
{
    public class PodcastService : IPodcastService
    {
        private readonly FaiEnglishContext _context;

        public PodcastService(FaiEnglishContext context)
        {
            _context = context;
        }

        public async Task<List<Podcast>> GetPostCard()
        {
            var podcasts = await _context.Podcasts
                .Where(p => p.Topic != null) // Lọc các podcast có chủ đề
                .OrderByDescending(p => p.CreatedAt) // Sắp xếp theo ngày tạo giảm dần
                .ToListAsync();
            if (podcasts == null || !podcasts.Any())
            {
                return new List<Podcast>(); // Trả về danh sách rỗng nếu không có podcast nào
            }
            return podcasts;
        }

        public async Task SavePodcastsAsync(List<Podcast> podcasts)
        {
            _context.Podcasts.AddRange(podcasts);
            await _context.SaveChangesAsync();
        }
    }
}
