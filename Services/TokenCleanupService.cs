using Microsoft.EntityFrameworkCore;
using stock_api.Models;

namespace stock_api.Services;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public TokenCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DbContexts>();
                var oldDate = DateTime.Now.AddDays(-3);

                var expiredTokens = await context.Refreshtokens
                    .Where(t => t.ExpiryDate < DateTime.Now || (t.IsRevoked && t.CreateDate < oldDate))
                    .ToListAsync(stoppingToken);

                if (expiredTokens.Any())
                {
                    context.Refreshtokens.RemoveRange(expiredTokens);
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}