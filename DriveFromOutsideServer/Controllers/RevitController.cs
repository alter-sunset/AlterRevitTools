using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriveFromOutsideServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RevitController : ControllerBase
    {

        [HttpGet]
        public IActionResult GetTask(int revitVersion)
        {
            return Ok();
        }
    }
}
