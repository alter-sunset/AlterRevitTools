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
        public IActionResult PostNewAssignment(Emperor emperor) //add some object as an argument that describes task
        {
            if (emperor is null) return BadRequest("Null reference");
            _db.Database.EnsureCreated();

            EmperorAssignment emp = new(emperor)
            {
                IssueTime = DateTime.Now,
                Status = AssignmentStatus.New
            };

            _db.Emperors.Add(emp);
            _db.SaveChanges();

            return Ok($"Assignment added at {emp.IssueTime}");
        }
    }
}