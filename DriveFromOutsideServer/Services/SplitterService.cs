using DriveFromOutsideServer.Configs;
using DriveFromOutsideServer.DB;
using Newtonsoft.Json;

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
                    string[] files = JsonConvert.DeserializeObject<IConfigEmperor>(emperor.Config).Files;

                    //deserialize config, replace files with file, serialize back
                    string newConfig = null;

                    IEnumerable<KingAssignment> kings = files.Select(e => new KingAssignment
                    {
                        Type = emperor.Type,
                        IssueTime = emperor.IssueTime,
                        Status = emperor.Status,
                        EmperorId = emperor.Id,
                        Version = emperor.Version,
                        //put method here
                        Config = newConfig
                    });

                    emperor.Status = AssignmentStatus.Open;

                    _db.AddRange(kings);
                }
                _db.SaveChanges();

                await Task.Delay(30000, stoppingToken);
            }
        }

        private string GetConfigForSingleFile(string config, AssignmentType type, string file)
        {

        }
    }
}