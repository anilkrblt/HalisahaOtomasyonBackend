using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;

[ApiController]
[Route("api/matches")]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _service;

    public MatchesController(IMatchService service)
    {
        _service = service;
    }

    /*────────  GET  ─────────────────────────────────*/

    // GET api/matches/55
    [HttpGet("{id:int}", Name = "GetMatchById")]
    public async Task<IActionResult> GetMatch(int id)
    {
        var match = await _service.GetMatchAsync(id, trackChanges: false);
        return Ok(match);
    }

    // GET api/matches/field/7
    [HttpGet("field/{fieldId:int}")]
    public async Task<IActionResult> GetMatchesByField(int fieldId)
    {
        var matches = await _service.GetMatchesByFieldIdAsync(fieldId, trackChanges: false);
        return Ok(matches);
    }

    // GET api/matches/team/12
    [HttpGet("team/{teamId:int}")]
    public async Task<IActionResult> GetMatchesByTeam(int teamId)
    {
        var matches = await _service.GetMatchesByTeamIdAsync(teamId, trackChanges: false);
        return Ok(matches);
    }

    /*────────  SCORE UPDATE  ───────────────────────*/

    // POST api/matches/55/score
    [HttpPost("{id:int}/score")]
    public async Task<IActionResult> UpdateScore(int id,
                                                 [FromBody] ScoreUpdateDto dto)
    {
        await _service.UpdateScoreAsync(id, dto);
        return NoContent();
    }
}
