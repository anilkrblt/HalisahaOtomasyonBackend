using System.Text;
using System.Text.Json;
using HalisahaOtomasyon.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;

[ApiController]
[Route("api/comments")]
public class CommentsController : ControllerBase
{
    private readonly IServiceManager _serviceManager;
    private readonly IConfiguration _config;

    public CommentsController(IServiceManager service, IConfiguration config)
    {
        _serviceManager = service;
        _config = config;
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


        var field = await _serviceManager.FieldService.GetFieldAsync(dto.FieldId, false);
        if (field?.OwnerId is not null && field.OwnerId != userId)
        {
            
            await _serviceManager.NotificationService.CreateNotificationAsync(new NotificationForCreationDto
            {
                UserId = field.OwnerId,
                Title = "Sahanıza Yorum Yapıldı",
                Content = created.Content ?? "",
                RelatedId = created.Id,
                RelatedType = "field-comment"
            });
        }

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

        var team = await _serviceManager.TeamService.GetTeamAsync(dto.ToTeamId, userId, false);
        if (team?.Members != null && !team.Members.Any(m => m.UserId == userId))
        {
            var ownerMember = team.Members.FirstOrDefault(m => m.IsCaptain || m.IsAdmin);

            if (ownerMember is not null)
            {
                await _serviceManager.NotificationService.CreateNotificationAsync(new NotificationForCreationDto
                {
                    UserId = ownerMember.UserId,
                    Title = "Takımınıza Yorum Yapıldı",
                    Content = "Bir kullanıcı takımınıza yorum yaptı.",
                    RelatedId = created.Id,
                    RelatedType = "team-comment"
                });
            }
        }


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
        if (dto.ToUserId != userId)
        {
            await _serviceManager.NotificationService.CreateNotificationAsync(new NotificationForCreationDto
            {
                UserId = dto.ToUserId,
                Title = "Profilinize Yorum Yapıldı",
                Content = "Bir kullanıcı sizin hakkınızda yorum yaptı.",
                RelatedId = created.Id,
                RelatedType = "user-comment"
            });
        }

        return CreatedAtRoute(
            routeName: "GetUserComment",
            routeValues: new { commentId = created.Id },
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



    [HttpPost("ai-analyze")]
    [Authorize]
    public async Task<IActionResult> AnalyzeCommentWithAI()
    {
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim is null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        // 1. Kullanıcıya ait sahalardaki ve tesislerdeki son 10 yorum
        var comments = await _serviceManager.CommentService.GetLast10CommentsForUserFieldsAsync(userId);

        if (comments == null || !comments.Any())
            return NotFound("Yorum bulunamadı.");

        var uniqueSuggestions = new HashSet<string>();

        var aiUrl = _config["AI:AnalyzeUrl"];

        using var client = new HttpClient();

        foreach (var comment in comments)
        {
            var payload = new
            {
                comment = comment.Content,
                rating = comment.Rating
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(aiUrl, content);
                if (!response.IsSuccessStatusCode)
                    continue;

                var responseBody = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(responseBody).RootElement;

                var issues = doc.GetProperty("detected_issues")
                                .EnumerateArray()
                                .Select(x => x.GetString());

                foreach (var issue in issues)
                {
                    if (!string.IsNullOrWhiteSpace(issue))
                        uniqueSuggestions.Add(issue!);
                }
            }
            catch
            {
                continue;
            }
        }

        return Ok(uniqueSuggestions.ToList());
    }

}
