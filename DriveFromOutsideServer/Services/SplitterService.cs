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
                    emperor.Status = AssignmentStatus.Open;

                    if (emperor.Type is AssignmentType.Transmit)
                    {
                        KingAssignment king = new()
                        {
                            EmperorId = emperor.Id,
                            Config = emperor.Config,
                            IssueTime = emperor.IssueTime,
                            Status = AssignmentStatus.New,
                            Type = emperor.Type,
                            Version = emperor.Version,
                        };
                        _db.Add(king);
                        continue;
                    }

                    //TODO: migrate case

                    string[] files = JsonConvert.DeserializeObject<IConfigEmperor>(emperor.Config).Files;

                    IEnumerable<KingAssignment> kings = files.Select(file => new KingAssignment
                    {
                        Type = emperor.Type,
                        IssueTime = emperor.IssueTime,
                        Status = emperor.Status,
                        EmperorId = emperor.Id,
                        Version = emperor.Version,
                        Config = CreateKingConfig(emperor.Type, emperor.Config, file)
                    });

                    _db.AddRange(kings);
                }
                _db.SaveChanges();

                await Task.Delay(30000, stoppingToken);
            }
        }

        private static string CreateKingConfig(AssignmentType assignmentType, string emperorConfig, string file)
            => assignmentType switch
            {
                AssignmentType.Detach => CreateConfig<DetachConfigEmperor, DetachConfigKing>(emperorConfig, file),
                AssignmentType.IFC => CreateConfig<IfcConfigEmperor, IfcConfigKing>(emperorConfig, file),
                AssignmentType.NWC => CreateConfig<NwcConfigEmperor, NwcConfigKing>(emperorConfig, file),
                AssignmentType.Update => CreateConfig<UpdateConfigEmperor, UpdateConfigKing>(emperorConfig, file),
                _ => throw new InvalidOperationException()
            };

        private static string CreateConfig<TEmperor, TKing>(string emperorConfig, string file)
            where TEmperor : IConfigEmperor, new()
            where TKing : IConfigKing, new()
        {
            TEmperor emperor = JsonConvert.DeserializeObject<TEmperor>(emperorConfig);
            TKing king = new() { File = file };
            king.InheritFromEmperor(emperor);
            return JsonConvert.SerializeObject(king);
        }
    }
}