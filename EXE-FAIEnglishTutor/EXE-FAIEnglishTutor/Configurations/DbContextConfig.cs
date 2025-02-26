using EXE_FAIEnglishTutor.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EXE_FAIEnglishTutor.Configurations
{
    public static class DbContextConfig
    {
        public static void AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FaiEnglishContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("FAI_ENGLISH"));
            });

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));


        }
    }
}
