using System.Data;
using stock_api.Repositories.Dapper;

namespace stock_api.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenCleanupService> _logger;

    public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // BackgroundService เป็น Singleton จึงต้องสร้าง scope เพื่อขอ IDbConnection (Scoped) ทุกรอบ
                using (var scope = _serviceProvider.CreateScope())
                {
                    var connection = scope.ServiceProvider.GetRequiredService<IDbConnection>();
                    var authRepository = new DapperAuthRepository(connection);

                    var deleted = await authRepository.DeleteExpiredRefreshTokensAsync(revokedOlderThanDays: 3);
                    _logger.LogInformation("TokenCleanupService: deleted {Count} expired refresh tokens", deleted);
                }
            }
            catch (Exception ex)
            {
                // กันไม่ให้ error ครั้งเดียว (เช่น DB ล่มชั่วคราว) ทำให้ Background Service หยุดทำงานทั้งหมด
                _logger.LogError(ex, "TokenCleanupService: failed to delete expired refresh tokens");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}