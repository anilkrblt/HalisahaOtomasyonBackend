using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;

[ApiController]
[Route("api/matches")]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _svc;           // DOĞRUDAN servis

    public MatchesController(IMatchService svc)    // ① sadece ihtiyacımız olan servisi enjekte ediyoruz
    {
        _svc = svc;
    }

    /*────────────── MATCH CRUD ──────────────*/

    // GET api/matches
    [HttpGet]
    public async Task<IActionResult> GetMatches() =>
        Ok(await _svc.GetAllMatchesAsync(trackChanges: false));

    // GET api/matches/5
    [HttpGet("{id:int}", Name = "GetMatchById")]
    public async Task<IActionResult> GetMatch(int id) =>
        Ok(await _svc.GetMatchAsync(id, trackChanges: false));

    // POST api/matches
    [HttpPost]
    public async Task<IActionResult> CreateMatch([FromBody] MatchForCreationDto dto)
    {
        var created = await _svc.CreateMatchAsync(dto);
        return CreatedAtRoute("GetMatchById", new { id = created.Id }, created);
    }

    // PUT api/matches/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMatch(int id, [FromBody] MatchForUpdateDto dto)
    {
        await _svc.UpdateMatchAsync(id, dto);
        return NoContent();
    }

    // DELETE api/matches/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMatch(int id)
    {
        await _svc.DeleteMatchAsync(id);
        return NoContent();
    }

    /*────────────── MATCH REQUESTS ──────────────*/

    // POST api/matches/requests
    [HttpPost("requests")]
    public async Task<IActionResult> CreateMatchRequest([FromBody] MatchRequestForCreationDto dto)
    {
        var created = await _svc.CreateMatchRequestAsync(dto);
        return CreatedAtAction(nameof(GetRequestsSentByUser), new { userId = created.FromUserId }, created);
    }

    // PUT api/match-requests/7   body: { "status": "Accepted" }
    [HttpPut("/api/match-requests/{requestId:int}")]
    public async Task<IActionResult> RespondMatchRequest(int requestId, [FromBody] MatchRequestRespondDto dto)
    {
        await _svc.RespondMatchRequestAsync(requestId, dto.Status);
        return NoContent();
    }

    // GET api/users/10/match-requests/sent
    [HttpGet("/api/users/{userId:int}/match-requests/sent")]
    public async Task<IActionResult> GetRequestsSentByUser(int userId) =>
        Ok(await _svc.GetRequestsSentByUserAsync(userId, trackChanges: false));

    // GET api/users/10/match-requests/received
    [HttpGet("/api/users/{userId:int}/match-requests/received")]
    public async Task<IActionResult> GetRequestsReceivedByUser(int userId) =>
        Ok(await _svc.GetRequestsReceivedByUserAsync(userId, trackChanges: false));

    // DELETE api/match-requests/7
    [HttpDelete("/api/match-requests/{requestId:int}")]
    public async Task<IActionResult> DeleteMatchRequest(int requestId)
    {
        await _svc.DeleteMatchRequestAsync(requestId);
        return NoContent();
    }
}
