using JobTracker.Api.Data;
using JobTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Api.Services
{
    public class DemoCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DemoCleanupService> _logger;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _cleanupInterval;
        private readonly bool _isEnabled;

        public DemoCleanupService(
            IServiceProvider serviceProvider,
            ILogger<DemoCleanupService> logger,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;

            _isEnabled = _configuration.GetValue<bool>("DemoMode:Enabled");
            var intervalMinutes = _configuration.GetValue<int>("DemoMode:CleanupIntervalMinutes");
            _cleanupInterval = TimeSpan.FromMinutes(intervalMinutes > 0 ? intervalMinutes : 60); // Default to 60 if missing
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_isEnabled)
            {
                _logger.LogInformation("Demo Mode is disabled. DemoCleanupService will not run.");
                return;
            }

            _logger.LogWarning("⚠️ Demo Mode is ENABLED. Data will be wiped every {Interval} minutes.", _cleanupInterval.TotalMinutes);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Wait for the interval
                    await Task.Delay(_cleanupInterval, stoppingToken);

                    await PerformCleanupAsync(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Graceful shutdown
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during Demo Mode cleanup.");
                }
            }
        }

        private async Task PerformCleanupAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🧹 Starting Demo Mode cleanup...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // 1. Delete all JobApplications
                // ExecuteDeleteAsync is efficient in EF Core 7+
                var count = await context.JobApplications.ExecuteDeleteAsync(cancellationToken);
                _logger.LogInformation("Deleted {Count} job applications.", count);

                // 2. Reseed Data
                if (_configuration.GetValue<bool>("DemoMode:SeedData"))
                {
                    await SeedDataAsync(context, cancellationToken);
                }
            }

            _logger.LogInformation("✅ Demo Mode cleanup completed. Database reset to seed state.");
        }

        private async Task SeedDataAsync(ApplicationDbContext context, CancellationToken cancellationToken)
        {
            var seedJobs = SeedDataHelper.GetSeedJobs();

            context.JobApplications.AddRange(seedJobs);
            await context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Reseeded database with {Count} sample jobs.", seedJobs.Count);
        }
    }
}
