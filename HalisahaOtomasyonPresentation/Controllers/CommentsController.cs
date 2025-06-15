using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _svc;

    public CommentsController(IServiceManager service)
    {
        _svc = service.CommentService;
    }

    [HttpGet("field/{fieldId:int}/comments", Name = "GetFieldComments")]
    public async Task<IActionResult> GetFieldComments([FromRoute] int fieldId)
    {
        var comments = await _svc.GetFieldCommentsAsync(fieldId, trackChanges: false);

        return Ok(comments);
    }

    [HttpGet("field/{fieldId:int}/comments/{commentId}", Name = "GetFieldComment")]
    public async Task<IActionResult> GetFieldComment([FromRoute] int fieldId, [FromRoute] int commentId)
    {
        var comments = await _svc.GetFieldCommentAsync(commentId, trackChanges: false);

        return Ok(comments);
    }

    [HttpPost("field")]
    [Authorize]
    public async Task<IActionResult> AddFieldComment([FromBody] FieldCommentForCreationDto dto)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var created = await _svc.AddFieldCommentAsync(dto, userId);


        return CreatedAtRoute(
            routeName: "GetFieldComment",
            routeValues: new
            {
                fieldId = dto.FieldId,
                commentId = created.Id
            },
            value: created);
    }

    [HttpPut("field/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateFieldComment([FromRoute] int commentId, [FromBody] FieldCommentForUpdateDto dto)
    {
        await _svc.UpdateFieldCommentAsync(commentId, dto);
        return NoContent();
    }

    [HttpDelete("field/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteFieldComment([FromRoute] int commentId)
    {
        await _svc.DeleteFieldCommentAsync(commentId);
        return NoContent();
    }

    [HttpGet("team/{teamId:int}/comments", Name = "GetTeamComments")]
    public async Task<IActionResult> GetTeamComments([FromRoute] int teamId) =>
        Ok(await _svc.GetTeamCommentsAsync(teamId, trackChanges: false));

    [HttpPost("team")]
    [Authorize]
    public async Task<IActionResult> AddTeamComment([FromBody] TeamCommentForCreationDto dto)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var created = await _svc.AddTeamCommentAsync(dto, userId);

        return CreatedAtRoute(
            routeName: "GetTeamComment",
            routeValues: new
            {
                teamId = dto.TeamId,
                commentId = created.Id
            },
            value: created);
    }

    [HttpPut("team/{teamId:int}/comment/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateTeamComment(
        [FromRoute] int teamId,
        [FromRoute] int commentId,
        [FromBody] TeamCommentForUpdateDto dto)
    {
        await _svc.UpdateTeamCommentAsync(commentId, dto);
        return NoContent();
    }

    [HttpDelete("team/{teamId:int}/comment/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteTeamComment(
        [FromRoute] int teamId,
        [FromRoute] int commentId)
    {
        await _svc.DeleteTeamCommentAsync(commentId);
        return NoContent();
    }

    [HttpGet("user/to/{userId:int}")]
    public async Task<IActionResult> GetCommentsAboutUser(int userId) =>
        Ok(await _svc.GetCommentsAboutUserAsync(userId, trackChanges: false));

    [HttpGet("user/from/{userId:int}")]
    public async Task<IActionResult> GetCommentsFromUser(int userId) =>
        Ok(await _svc.GetCommentsFromUserAsync(userId, trackChanges: false));

    [HttpGet("user/comment/{id:int}", Name = "GetUserComment")]
    public async Task<IActionResult> GetUserComment(int id)
    {
        var comment = await _svc.GetUserCommentAsync(id, trackChanges: false);
        return comment is null ? NotFound() : Ok(comment);
    }

    [HttpPost("user")]
    [Authorize]
    public async Task<IActionResult> AddUserComment([FromBody] UserCommentForCreationDto dto)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var created = await _svc.AddUserCommentAsync(dto, userId);

        return CreatedAtRoute(
            routeName: "GetUserComment",
            routeValues: new { id = created.Id },
            value: created);
    }

    [HttpPut("user/comment/{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateUserComment([FromRoute] int id, [FromBody] UserCommentForUpdateDto dto)
    {
        await _svc.UpdateUserCommentAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("user/comment/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserComment(int id)
    {
        await _svc.DeleteUserCommentAsync(id);
        return NoContent();
    }
}
