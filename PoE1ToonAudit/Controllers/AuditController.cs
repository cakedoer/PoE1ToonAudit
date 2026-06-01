using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using PoE1ToonAudit.Dao;
using PoE1ToonAudit.Models;
using PoE1ToonAudit.Services;

namespace PoE1ToonAudit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditController(PoeApiDao dao, ToonAuditService auditService) : ControllerBase
{
    // get gear from dao
    [HttpGet("check-gear")]
    public async Task<IActionResult> CheckGear()
    {
        PoeToon data;
        try
        {
            data = await dao.FetchCharacterItemsAsync();
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(502, ex.Message);
        }

        // give deserialized gear to the audit logic and return the result
        PoeToonAuditResult result = auditService.Audit(data);
        return Ok(result);
    }
}