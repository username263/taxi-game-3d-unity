using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaxiGame3D.Server.Controllers;

[Route("game/[controller]")]
[ApiController]
public class TemplateController : ControllerBase
{
    [HttpPost("versions")]
    public async Task<ActionResult> GetVersions()
    {
        return Ok();
    }

}
