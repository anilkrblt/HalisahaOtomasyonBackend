using System.Security.Claims;
using Entities.Models;
using HalisahaOtomasyon.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<IActionResult> GetTeams([FromQuery] string? city, [FromQuery] string? teamName)
    {
        var teams = await _serviceManager.TeamService.GetTeamsAsync(city, teamName, false);
        return Ok(teams);
    }

    [HttpGet("{id:int}", Name = "GetTeam")]
    public async Task<IActionResult> GetTeam([FromRoute(Name = "id")] int id)
    {
        var team = await _serviceManager.TeamService.GetTeamAsync(id, false);
        return Ok(team);
    }

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateTeam([FromBody] TeamForCreationDto dto)
    {
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
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTeam([FromRoute(Name = "id")] int id, [FromForm] TeamForUpdateDto dto)
    {
        await _serviceManager.TeamService.UpdateTeamAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTeam([FromRoute(Name = "id")] int id)
    {
        await _serviceManager.TeamService.DeleteTeamAsync(id);
        return NoContent();
    }

    [HttpGet("{teamId:int}/members")]
    public async Task<IActionResult> GetMembers([FromRoute(Name = "teamId")] int teamId)
    {
        var members = await _serviceManager.TeamService.GetMembersAsync(teamId, false);
        return Ok(members);
    }

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPost("{teamId:int}/members")]
    public async Task<IActionResult> AddMembers([FromRoute(Name = "teamId")] int teamId, [FromBody] List<TeamMemberDtoForAdd> dtos)
    {
        await _serviceManager
              .TeamService
              .AddMembersAsync(teamId, dtos);
        return NoContent();
    }

    [HttpDelete("{teamId:int}/members/{userId:int}")]
    public async Task<IActionResult> RemoveMember([FromRoute(Name = "teamId")] int teamId, [FromRoute(Name = "userId")] int userId)
    {
        await _serviceManager.TeamService.RemoveMemberAsync(teamId, userId);
        return NoContent();
    }

    [HttpPut("{teamId:int}/members/{userId:int}")]
    public async Task<IActionResult> UpdateTeamMember([FromRoute(Name = "teamId")] int teamId, 
        [FromRoute(Name = "userId")] int userId, 
        [FromBody] TeamMemberDtoForUpdateAdminAndCaptain teamMemberDto)
    {
        var member = await _serviceManager.TeamService.SetAdminAndCaptain(teamId, userId, teamMemberDto);
        return Ok(member);
    }

    [HttpPost("{teamId:int}/join-requests")]
    [Authorize]
    public async Task<IActionResult> CreateJoinRequest([FromRoute(Name = "teamId")] int teamId)
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

    [HttpGet("{teamId:int}/join-requests", Name = "GetTeamJoinRequests")]
    public async Task<IActionResult> GetTeamJoinRequests([FromRoute(Name = "teamId")] int teamId)
    {
        var list = await _serviceManager.TeamService.GetTeamJoinRequestsAsync(teamId, trackChanges: false);
        return Ok(list);
    }

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

    [HttpPut("/api/join-requests/{requestId:int}")]
    [Authorize]
    public async Task<IActionResult> RespondJoinRequest([FromRoute(Name = "requestId")] int requestId, [FromBody] RequestStatus status)
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
