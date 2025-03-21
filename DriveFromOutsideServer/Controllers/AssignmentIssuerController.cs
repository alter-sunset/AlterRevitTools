using DriveFromOutsideServer.Configs;
using DriveFromOutsideServer.DB;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DriveFromOutsideServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentIssuerController(ILogger<AssignmentIssuerController> logger, AssignmentContext db) : ControllerBase
    {
        private const int _lastVersion = 2025;

        private ILogger<AssignmentIssuerController> _logger = logger;
        private readonly AssignmentContext _db = db;

        [HttpPost("AddTransmitJob")]
        public IActionResult PostTransmitAssignment(TransmitConfig config, int version) => AddAssignment(AssignmentType.Transmit, config, version);

        [HttpPost("AddDetachJob")]
        public IActionResult PostDetachAssignment(DetachConfigEmperor config, int version) => AddAssignment(AssignmentType.Detach, config, version);

        [HttpPost("AddNwcJob")]
        public IActionResult PostNwcAssignment(NwcConfigEmperor config, int version) => AddAssignment(AssignmentType.NWC, config, version);

        [HttpPost("AddIfcJob")]
        public IActionResult PostIfcAssignment(IfcConfigEmperor config, int version) => AddAssignment(AssignmentType.IFC, config, version);

        [HttpPost("AddMigrateJob")]
        public IActionResult PostMigrateAssignment(MigrateConfig config, int version) => AddAssignment(AssignmentType.Migrate, config, version);

        [HttpPost("AddUpdateJob")]
        public IActionResult PostUpdateAssignment(UpdateConfigEmperor config, int version) => AddAssignment(AssignmentType.Update, config, version);

        private IActionResult AddAssignment<T>(AssignmentType type, T config, int version)
        {
            if (config is null) return BadRequest("Null reference");
            if (version < 2019 || version > _lastVersion) return BadRequest($"Wrong Revit version. Must be between 2019 and {_lastVersion}");

            _db.Database.EnsureCreated();

            EmperorAssignment emperor = new()
            {
                Type = type,
                Version = version,
                Config = JsonConvert.SerializeObject(config),
                IssueTime = DateTime.Now,
                Status = AssignmentStatus.New
            };

            _db.Emperors.Add(emperor);
            _db.SaveChanges();

            return Ok($"Assignment to {Enum.GetName(type)} files in Revit {version} added at {emperor.IssueTime}");
        }
    }
}