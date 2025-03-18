namespace DriveFromOutsideServer.Services
{
    public class DistributionService(ILogger<DistributionService> logger) : BackgroundService
    {
        private readonly ILogger<DistributionService> _logger = logger;
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //get kings
                //
            }
            return Task.CompletedTask;
        }
    }
}