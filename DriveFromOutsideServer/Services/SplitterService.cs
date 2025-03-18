using DriveFromOutsideServer.DB;

namespace DriveFromOutsideServer.Services
{
    public class SplitterService(ILogger<SplitterService> logger, AssignmentContext db) : BackgroundService
    {
        private readonly ILogger<SplitterService> _logger = logger;
        private readonly AssignmentContext _db = db;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                IQueryable<EmperorAssignment> emperors = _db.Emperors
                    .Where(e => e.Status == AssignmentStatus.New);

                foreach (EmperorAssignment emperor in emperors)
                {
                    //create kings
                }

                await Task.Delay(30000, stoppingToken);
            }
        }
    }
}