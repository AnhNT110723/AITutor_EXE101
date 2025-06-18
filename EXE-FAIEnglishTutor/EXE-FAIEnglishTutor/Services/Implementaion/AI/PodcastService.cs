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

        //public async Task<List<Podcast>> GetPostCard()
        //{
        //    return await _context.Podcasts.ToListAsync();
        //}

        //public async Task SavePodcastsAsync(List<Podcast> podcasts)
        //{
        //    _context.Podcasts.AddRange(podcasts);
        //    await _context.SaveChangesAsync();
        //}
    }
}
