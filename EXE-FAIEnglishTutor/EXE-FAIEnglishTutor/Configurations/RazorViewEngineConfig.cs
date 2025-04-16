using Microsoft.AspNetCore.Mvc.Razor;

namespace EXE_FAIEnglishTutor.Configurations
{
    public static class RazorViewEngineConfig
    {
        public static void ConfigureRazorViewEngine(this IServiceCollection services)
        {
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationFormats.Clear();
                options.ViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                options.ViewLocationFormats.Add("/Areas/{2}/Views/Shared/{0}.cshtml");
                options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
                options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
            });
        }
    }
}
