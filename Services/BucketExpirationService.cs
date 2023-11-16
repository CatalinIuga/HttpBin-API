using HttpBin.Models;

namespace HttpBin.Services;

public class BucketExpirationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BucketExpirationBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HttpBinDbContext>();
                var expiredBuckets = dbContext.Buckets
                    .Where(b => b.UpdatedAt.AddSeconds(b.TimeToLive) < DateTime.UtcNow)
                    .ToList();

                foreach (var bucket in expiredBuckets)
                {
                    Console.WriteLine($"Deleted bucket: {bucket.BucketId}");
                    dbContext.Buckets.Remove(bucket);
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
