using Microsoft.Extensions.Configuration;

namespace EXE_FAIEnglishTutor.Configurations
{
    public static class AppConstantsConfig
    {
        public static void InitializeConstants(this IConfiguration configuration)
        {
            var configuredBaseUrl = configuration["App:BaseUrl"];
            if (!string.IsNullOrEmpty(configuredBaseUrl))
            {
                Common.Constants.BASE_URL = configuredBaseUrl;
            }
        }
    }
}
