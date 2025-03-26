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
        private const int MAX_INSTANCES = 5;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                IOrderedQueryable<KingAssignment> assignments = _db.Kings
                    .Where(e => e.Status == AssignmentStatus.New)
                    .OrderByDescending(e => e.Version);

                //should i move initialization to class?
                SemaphoreSlim semaphoreAssignment = new(MAX_INSTANCES);
                SemaphoreSlim semaphoreRevit = new(MAX_INSTANCES);

                ConcurrentQueue<Task> tasks = new();
                foreach (KingAssignment assignment in assignments)
                {
                    Task task = new(() =>
                    {
                        semaphoreAssignment.Wait();
                        //check for correct version and available slot
                        if (_revitInstances.Count < MAX_INSTANCES)
                        {
                            _revitInstances.Add(new RevitInstance(assignment.Version, semaphoreRevit));
                        }
                    });
                }

                //add couple semaphores: 1st for assignmeents, 2nd for revits
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