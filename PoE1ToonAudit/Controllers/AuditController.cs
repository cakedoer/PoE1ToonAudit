using Microsoft.AspNetCore.Mvc;

namespace PoE1ToonAudit.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AuditController(IConfiguration config, ILogger<AuditController> logger) : ControllerBase // does dep inject automatically
{
    [HttpGet("test")]
    public IActionResult Test()
    {
        logger.LogInformation("Test hit");
        return Ok("Success");
    }
}