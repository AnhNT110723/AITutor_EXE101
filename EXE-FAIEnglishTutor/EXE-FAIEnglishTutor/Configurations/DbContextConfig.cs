using EXE_FAIEnglishTutor.Areas.Mentee.Controllers;
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
                options.UseSqlServer(configuration.GetConnectionString("OLS"));
            });

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            // Đọc cấu hình Azure Translator
            services.Configure<AzureTranslatorConfig>(configuration.GetSection("AzureTranslator"));


        }
    }
}
