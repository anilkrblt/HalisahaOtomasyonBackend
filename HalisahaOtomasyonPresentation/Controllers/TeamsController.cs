using System.Security.Claims;
using HalisahaOtomasyon.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;

[ApiController]
[Route("api/teams")]
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

    [Authorize]
    [HttpGet("{teamId:int}", Name = "GetTeam")]
    public async Task<IActionResult> GetTeam([FromRoute(Name = "teamId")] int teamId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();

        var reviewerId = int.Parse(userIdClaim);

        var team = await _serviceManager.TeamService.GetTeamAsync(teamId, reviewerId, false);
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

        var created = await _serviceManager.TeamService.CreateTeamAsync(dto, creatorUserId);

        return CreatedAtRoute("GetTeam", new { teamId = created.Id }, created);
    }

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPut("{teamId:int}")]
    public async Task<IActionResult> UpdateTeam([FromRoute(Name = "teamId")] int teamId, [FromBody] TeamForUpdateDto dto)
    {
        await _serviceManager.TeamService.UpdateTeamAsync(teamId, dto);
        return NoContent();
    }

    [HttpDelete("{teamId:int}")]
    public async Task<IActionResult> DeleteTeam([FromRoute(Name = "teamId")] int teamId)
    {
        await _serviceManager.TeamService.DeleteTeamAsync(teamId);
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

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPut("{teamId:int}/members/{userId:int}")]
    public async Task<IActionResult> UpdateTeamMember([FromRoute(Name = "teamId")] int teamId,
    [FromRoute(Name = "userId")] int userId,
    [FromBody] TeamMemberDtoForUpdateAdminAndCaptain teamMemberDto)
    {
        var member = await _serviceManager.TeamService.SetAdminAndCaptain(teamId, userId, teamMemberDto);
        return Ok(member);
    }

    [HttpDelete("{teamId:int}/members/{userId:int}")]
    public async Task<IActionResult> RemoveMember([FromRoute(Name = "teamId")] int teamId, [FromRoute(Name = "userId")] int userId)
    {
        await _serviceManager.TeamService.RemoveMemberAsync(teamId, userId);
        return NoContent();
    }

    [HttpGet("{teamId:int}/join-requests", Name = "GetTeamJoinRequests")]
    public async Task<IActionResult> GetTeamJoinRequests([FromRoute(Name = "teamId")] int teamId)
    {
        var list = await _serviceManager.TeamService.GetTeamJoinRequestsAsync(teamId, trackChanges: false);
        return Ok(list);
    }

    [HttpGet("user-join-requests", Name = "GetMyJoinRequests")]
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

    [HttpPost("{teamId:int}/join-requests")]
    [Authorize]
    public async Task<IActionResult> CreateJoinRequest([FromRoute(Name = "teamId")] int teamId)
    {
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

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPut("{teamId:int}/join-requests/{requestId:int}")]
    [Authorize]
    public async Task<IActionResult> RespondJoinRequest([FromRoute(Name = "teamId")] int teamId,
        [FromRoute(Name = "requestId")] int requestId, 
        [FromBody] TeamJoinRequestDtoForUpdate dto)
    {
        var responderClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? User.FindFirst("id")?.Value;
        if (responderClaim is null)
            return Unauthorized();
        var responderId = int.Parse(responderClaim);

        await _serviceManager.TeamService
            .RespondJoinRequestAsync(teamId, requestId, dto, responderId);

        return NoContent();
    }
}
