using System.Security.Claims;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    public readonly IServiceManager _serviceManager;
    public TeamsController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }
    /*───────────────────── TEAM CRUD ─────────────────────*/

    // GET api/teams?city=İzmir&search=kartal
    [HttpGet]
    public async Task<IActionResult> GetTeams([FromQuery] string? city, [FromQuery] string? search)
    {
        var list = city is { Length: > 0 }
            ? await _serviceManager.TeamService.GetTeamsByCityAsync(city, false)
            : search is { Length: > 0 }
                ? await _serviceManager.TeamService.SearchTeamsByNameAsync(search, false)
                : await _serviceManager.TeamService.GetAllTeamsAsync(false);

        return Ok(list);
    }

    [HttpGet("{id:int}", Name = "GetTeam")]
    public async Task<IActionResult> GetTeam(int id)
    {
        var team = await _serviceManager.TeamService.GetTeamAsync(id, false);
        return Ok(team);
    }


    // HalisahaOtomasyonPresentation/Controllers/TeamsController.cs

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTeam([FromBody] TeamForCreationDto dto)
    {
        if (dto is null)
            return BadRequest();

        // 1) Token’dan userId al
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();
        var creatorUserId = int.Parse(userIdClaim);

        // 2) Servisi çağır
        var created = await _serviceManager.TeamService.CreateTeamAsync(dto, creatorUserId);
        /*
        if (dto.LogoFile == null) return BadRequest("LogoFile is required.");
        await _serviceManager.TeamService.SetTeamLogoAsync(created.Id, dto.LogoFile);
*/
        // 3) 201 ve Location header
        return CreatedAtRoute("GetTeam", new { id = created.Id }, created);
    }

/*
// update yaz
    // Yeni logo yükleme endpoint
    [HttpPost("{id:int}/logo")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadTeamLogo([FromRoute] int id, [FromForm] TeamLogoUploadDto dto)
    {
        if (dto.LogoFile == null) return BadRequest("LogoFile is required.");
        await _serviceManager.TeamService.SetTeamLogoAsync(id, dto.LogoFile);
        return NoContent();
    }

*/
    // PUT api/teams/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTeam(int id, [FromBody] TeamForUpdateDto dto)
    {
        await _serviceManager.TeamService.UpdateTeamAsync(id, dto);
        return NoContent();
    }

    // DELETE api/teams/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTeam(int id)
    {
        await _serviceManager.TeamService.DeleteTeamAsync(id);
        return NoContent();
    }

    /*───────────────────── TEAM MEMBERS ──────────────────*/

    // GET api/teams/5/members
    [HttpGet("{teamId:int}/members")]
    public async Task<IActionResult> GetMembers(int teamId)
    {
        var members = await _serviceManager.TeamService.GetMembersAsync(teamId, false);
        return Ok(members);
    }


    // POST api/teams/5/members
    [HttpPost("{teamId:int}/members")]
    public async Task<IActionResult> AddMember(int teamId, [FromBody] TeamMemberForAddDto dto)
    {
        // 1) Gövde boş mu?
        if (dto == null)
            return BadRequest("Request body cannot be empty.");

        // 2) Model valid mi?
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _serviceManager
              .TeamService
              .AddMemberAsync(teamId,
                              userId: dto.UserId,
                              pos: dto.Position,
                              isCaptain: dto.IsCaptain);
        return NoContent();
    }

    // DELETE api/teams/5/members/10
    [HttpDelete("{teamId:int}/members/{userId:int}")]
    public async Task<IActionResult> RemoveMember(int teamId, int userId)
    {
        await _serviceManager.TeamService.RemoveMemberAsync(teamId, userId);
        return NoContent();
    }

    /*───────────────────── JOIN REQUESTS ───────────────────*/

    // POST api/teams/5/join-requests
    [HttpPost("{teamId:int}/join-requests")]
    [Authorize]
    public async Task<IActionResult> CreateJoinRequest([FromRoute] int teamId)
    {
        // önce standart NameIdentifier’a bakalım, yoksa "id" claim’ine
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);
        var created = await _serviceManager.TeamService.CreateJoinRequestAsync(teamId, userId);

        return CreatedAtRoute(
            routeName: "GetTeamJoinRequests",
            routeValues: new { teamId },
            value: created
        );
    }


    // GET api/teams/5/join-requests
    [HttpGet("{teamId:int}/join-requests", Name = "GetTeamJoinRequests")]
    public async Task<IActionResult> GetTeamJoinRequests([FromRoute] int teamId)
    {
        var list = await _serviceManager.TeamService.GetTeamJoinRequestsAsync(teamId, trackChanges: false);
        return Ok(list);
    }

    // GET api/teams/join-requests  <-- kendi isteklerini listelemek için
    [HttpGet("join-requests", Name = "GetMyJoinRequests")]
    [Authorize]
    public async Task<IActionResult> GetUserJoinRequests()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);
        var list = await _serviceManager.TeamService.GetUserJoinRequestsAsync(userId, trackChanges: false);
        return Ok(list);
    }

    // PUT api/join-requests/7

    [HttpPut("/api/join-requests/{requestId:int}")]
    [Authorize]
    public async Task<IActionResult> RespondJoinRequest([FromRoute] int requestId, [FromBody] RequestStatus status)
    {
        // JWT'den respondent userId alıyoruz
        var responderClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("id")?.Value;
        if (responderClaim is null)
            return Unauthorized();
        var responderId = int.Parse(responderClaim);

        // Servise requestId, status ve responderId gönderiliyor
        await _serviceManager.TeamService
            .RespondJoinRequestAsync(requestId, status, responderId);

        return NoContent();
    }

}
