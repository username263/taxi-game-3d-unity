using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Nodes;
using TaxiGame3D.Server.Services;

namespace TaxiGame3D.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class TemplateController : ControllerBase
{
    readonly TemplateService service;

    public TemplateController(TemplateService service)
    {
        this.service = service;
    }

    [HttpGet("versions")]
    public async Task<ActionResult> GetVersions()
    {
        var versions = await service.GetVersions();
        return Ok(versions);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult> Get(string name)
    {
        var templates = await service.Get(name);
        if (templates == null)
            return NotFound();
        return Ok(templates);
    }

    [HttpPut("{name}")]
    public async Task<ActionResult> Update(string name, [FromBody] JsonArray datas)
    {
        var version = await service.Update(name, datas);
        return Ok(new { Version = version } );
    }

    [HttpDelete("{name}")]
    public async Task<ActionResult> Delete(string name)
    {
        await service.Delete(name);
        return NoContent();
    }
}
