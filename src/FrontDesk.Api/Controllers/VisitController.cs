using FrontDesk.Api.Services;
using FrontDesk.Shared.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FrontDesk.Api.Controllers;

[Route("api/visit")]
[ApiController]
public class VisitController : ControllerBase
{
    private readonly IVisitService _visitService;

    public VisitController(IVisitService visitService)
    {
        _visitService = visitService;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessVisitAsync([FromBody] VisitRequest req)
    {
        return Ok(await _visitService.ProcessVisitAsync(req));
    }
}
