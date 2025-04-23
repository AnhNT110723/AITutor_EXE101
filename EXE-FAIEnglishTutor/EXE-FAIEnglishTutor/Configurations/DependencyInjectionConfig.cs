using EXE_FAIEnglishTutor.Helpers;
using EXE_FAIEnglishTutor.Mail;
using EXE_FAIEnglishTutor.Repositories.Implementation;
using EXE_FAIEnglishTutor.Repositories.Interface;
using EXE_FAIEnglishTutor.Services.Implementaion;
using EXE_FAIEnglishTutor.Services.Interface;
using EXE_FAIEnglishTutor.Services.Interface.AI;
using EXE_FAIEnglishTutor.Services.Implimentaion.AI;
using EXE_FAIEnglishTutor.Repositories.Interface.Mentee;
using EXE_FAIEnglishTutor.Repositories.Implementation.Mentee;
using EXE_FAIEnglishTutor.Services.Interface.Mentee;
using EXE_FAIEnglishTutor.Services.Implementaion.Mentee;

namespace EXE_FAIEnglishTutor.Configurations
{
    public static class  DependencyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            // Thêm Dependency Injection

            //Login
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IRoleRepository, RoleRepositoryImpl>();
            services.AddScoped<IRegisterUserService, RegisterUserServiceImpl>();
            services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();
            services.AddScoped<IVerificationTokenRepository, VerificationTokenRepositoryImpl>();
            services.AddScoped<IVerificationTokenService, VerificationTokenServiceImpl>();
            services.AddScoped<IForgotPasswordService, ForgotPasswordServiceImpl>();
            services.AddTransient<EmailSendVetification>();

            // Chat bot - Sử dụng AddHttpClient thay vì AddScoped
            services.AddHttpClient<IChatBotService, ChatBotService>();


            //Profile
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IProfileService, ProfileService>();


            //UpLoad File
            services.AddScoped<IFileUploadService, FileUploadService>();


            //Toeic test
            services.AddScoped<IToeicTestService, ToeicTestService>();
            //Exam
            services.AddScoped<IExamRepositoy,ExamRepository>();

        }




    }
}
