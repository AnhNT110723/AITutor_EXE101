
using EXE_FAIEnglishTutor.Services.Interface;

namespace EXE_FAIEnglishTutor.Services.Implementaion
{
    public class AccountExpiryService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        public AccountExpiryService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //await DowngradeExpiredAccounts();
                await Task.Delay(_checkInterval, stoppingToken); // Chờ đến lần kiểm tra tiếp theo
            }
        }

        private async Task DowngradeExpiredAccounts()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
                var users = await userService.GetAllUsersAsync(); // Lấy tất cả người dùng

                foreach (var user in users.Where(u => u.UpgradeLevel > 0 && u.ExpiryDate <= DateTime.UtcNow))
                {
                    user.UpgradeLevel = 0; // Đặt lại trạng thái chưa nâng cấp
                    user.ExpiryDate = null; // Xóa ngày hết hạn
                    await userService.SaveChangeAsync(user);
                }
            }
        }
    }
}
