using HalisahaOtomasyon.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public CommentsController(IServiceManager service)
    {
        _serviceManager = service;
    }

    [HttpGet("fields/{fieldId:int}", Name = "GetFieldComments")]
    public async Task<IActionResult> GetFieldComments([FromRoute(Name = "fieldId")] int fieldId)
    {
        var comments = await _serviceManager.CommentService
            .GetFieldCommentsAsync(fieldId, trackChanges: false);

        return Ok(comments);
    }

    [HttpGet("field-comments/{commentId:int}", Name = "GetFieldComment")]
    public async Task<IActionResult> GetFieldComment([FromRoute(Name = "commentId")] int commentId)
    {
        var comments = await _serviceManager.CommentService.GetFieldCommentAsync(commentId, trackChanges: false);

        return Ok(comments);
    }

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPost("field-comments")]
    [Authorize]
    public async Task<IActionResult> AddFieldComment([FromBody] FieldCommentForCreationDto dto)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var created = await _serviceManager.CommentService.AddFieldCommentAsync(dto, userId);

        return CreatedAtRoute(
            routeName: "GetFieldComment",
            routeValues: new
            {
                commentId = created.Id
            },
            value: created);
    }

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPut("field-comments/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateFieldComment([FromRoute(Name = "commentId")] int commentId, 
        [FromBody] FieldCommentForUpdateDto dto)
    {
        await _serviceManager.CommentService.UpdateFieldCommentAsync(commentId, dto);
        return NoContent();
    }

    [HttpDelete("field-comments/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteFieldComment([FromRoute(Name = "commentId")] int commentId)
    {
        await _serviceManager.CommentService.DeleteFieldCommentAsync(commentId);
        return NoContent();
    }

    [HttpGet("teams/{teamId:int}", Name = "GetTeamComments")]
    public async Task<IActionResult> GetTeamComments([FromRoute(Name = "teamId")] int teamId)
    {
        var teamComments = await _serviceManager.CommentService.GetTeamCommentsAsync(teamId, trackChanges: false);
        return Ok(teamComments);
    }

    [HttpGet("team-comments/{commentId:int}", Name = "GetTeamComment")]
    public async Task<IActionResult> GetTeamComment([FromRoute(Name = "commentId")] int commentId)
    {
        var teamComment = await _serviceManager.CommentService.GetTeamCommentAsync(commentId, trackChanges: false);
        return Ok(teamComment);
}

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPost("team-comments")]
    [Authorize]
    public async Task<IActionResult> AddTeamComment([FromBody] TeamCommentForCreationDto dto)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var created = await _serviceManager.CommentService.AddTeamCommentAsync(dto, userId);

        return CreatedAtRoute(
            routeName: "GetTeamComment",
            routeValues: new
            {
                commentId = created.Id,
            },
            value: created);
    }

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPut("team-comments/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateTeamComment([FromRoute] int commentId,
        [FromBody] TeamCommentForUpdateDto dto)
    {
        await _serviceManager.CommentService.UpdateTeamCommentAsync(commentId, dto);
        return NoContent();
    }

    [HttpDelete("team-comments/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteTeamComment([FromRoute] int commentId)
    {
        await _serviceManager.CommentService.DeleteTeamCommentAsync(commentId);
        return NoContent();
    }

    [HttpGet("users/to/{userId:int}")]
    public async Task<IActionResult> GetCommentsAboutUser([FromRoute(Name = "userId")] int userId)
    {
        var comments = await _serviceManager.CommentService.GetCommentsAboutUserAsync(userId, trackChanges: false);
        return Ok(comments);
    }

    [HttpGet("users/from/{userId:int}")]
    public async Task<IActionResult> GetCommentsFromUser([FromRoute(Name = "userId")] int userId)
    {
        var comments = await _serviceManager.CommentService.GetCommentsFromUserAsync(userId, trackChanges: false);
        return Ok(comments);
    }

    [HttpGet("user-comments/{commentId:int}", Name = "GetUserComment")]
    public async Task<IActionResult> GetUserComment([FromRoute(Name = "commentId")] int commentId)
    {
        var comment = await _serviceManager.CommentService.GetUserCommentAsync(commentId, trackChanges: false);
        return Ok(comment);
    }

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPost("user-comments")]
    [Authorize]
    public async Task<IActionResult> AddUserComment([FromBody] UserCommentForCreationDto dto)
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();
        var userId = int.Parse(userIdClaim);

        var created = await _serviceManager.CommentService.AddUserCommentAsync(dto, userId);

        return CreatedAtRoute(
            routeName: "GetUserComment",
            routeValues: new { commentId = created.Id},
            value: created);
    }

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPut("user-comments/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateUserComment([FromRoute(Name = "commentId")] int commentId, [FromBody] UserCommentForUpdateDto dto)
    {
        await _serviceManager.CommentService.UpdateUserCommentAsync(commentId, dto);
        return NoContent();
    }

    [HttpDelete("user-comments/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteUserComment([FromRoute(Name = "commentId")] int commentId)
    {
        await _serviceManager.CommentService.DeleteUserCommentAsync(commentId);
        return NoContent();
    }
}
