using DriveFromOutsideServer.DB;
using Microsoft.AspNetCore.Mvc;

namespace DriveFromOutsideServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentIssuerController(ILogger<AssignmentIssuerController> logger, AssignmentContext db) : ControllerBase
    {
        private ILogger<AssignmentIssuerController> _logger = logger;
        private readonly AssignmentContext _db = db;

        [HttpPost]
        public IActionResult PostNewAssignment(Assignment assignment)
        {
            if (assignment is null) return BadRequest("Null reference");
            _db.Database.EnsureCreated();

            EmperorAssignment emperor = new(assignment)
            {
                IssueTime = DateTime.Now,
                Status = AssignmentStatus.New
            };

            _db.Emperors.Add(emperor);
            _db.SaveChanges();

            return Ok($"Assignment added at {emperor.IssueTime}");
        }
    }
}