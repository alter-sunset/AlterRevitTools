using DriveFromOutsideServer.DB;
using DriveFromOutsideServer.Revit;
using System.Collections.Concurrent;

namespace DriveFromOutsideServer.Services
{
    public class DistributionService(ILogger<DistributionService> logger, AssignmentContext db) : BackgroundService
    {
        private readonly ILogger<DistributionService> _logger = logger;
        private readonly AssignmentContext _db = db;
        private ConcurrentBag<RevitInstance> _revitInstances = [];
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                IQueryable<KingAssignment> assignments = _db.Kings.Where(e => e.Status == AssignmentStatus.New);



                //add couple semaphores 1st for assignmeents, 2nd for revits
                //use SemaphoreSlim
                //get kings
                //run revits
                //???
                //PROFIT!!!

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}