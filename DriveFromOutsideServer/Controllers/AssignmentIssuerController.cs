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
        private ILogger<AssignmentIssuerController> _logger = logger;
        private readonly AssignmentContext _db = db;

        [HttpPost("AddTransmitJob")]
        public IActionResult PostTransmitAssignment(TransmitConfig config) => AddAssignment(AssignmentType.Transmit, config);

        [HttpPost("AddDetachJob")]
        public IActionResult PostDetachAssignment(DetachConfigEmperor config) => AddAssignment(AssignmentType.Detach, config);

        [HttpPost("AddNwcJob")]
        public IActionResult PostNwcAssignment(NwcConfigEmperor config) => AddAssignment(AssignmentType.NWC, config);

        [HttpPost("AddIfcJob")]
        public IActionResult PostIfcAssignment(IfcConfigEmperor config) => AddAssignment(AssignmentType.IFC, config);

        [HttpPost("AddMigrateJob")]
        public IActionResult PostMigrateAssignment(MigrateConfig config) => AddAssignment(AssignmentType.Migrate, config);

        [HttpPost("AddUpdateJob")]
        public IActionResult PostUpdateAssignment(UpdateConfigEmperor config) => AddAssignment(AssignmentType.Update, config);

        private IActionResult AddAssignment<T>(AssignmentType type, T config)
        {
            if (config is null) return BadRequest("Null reference");

            _db.Database.EnsureCreated();

            EmperorAssignment emperor = new()
            {
                Type = type,
                Config = JsonConvert.SerializeObject(config),
                IssueTime = DateTime.Now,
                Status = AssignmentStatus.New
            };

            _db.Emperors.Add(emperor);
            _db.SaveChanges();

            return Ok($"Assignment to {Enum.GetName(type)} files added at {emperor.IssueTime}");
        }
    }
}