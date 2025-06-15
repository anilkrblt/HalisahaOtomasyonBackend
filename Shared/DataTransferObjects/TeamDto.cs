using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Entities.Models;
using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects;

public record TeamDto(
    int Id,
    string Name,
    string LogoUrl,
    string City,
    string Town,
    DateTime CreatedAt,
    int MatchPlayed,
    int MatchWon,
    int MatchDrawn,
    int MatchLost,
    string Content,
    double AvgRating,
    IEnumerable<TeamMemberDto> Members);

public record TeamForCreationDto(
    string Name,
    string City,
    string Town,
    string Content,
    string LogoUrl);

public class TeamForUpdateDto
{
    public string Name { get; set; }
    public string City { get; set; }
    public string Town { get; set; }
    public string Content { get; set; }
    public IFormFile Logo {  get; set; }
}

public class TeamLogoUploadDto
{
    [Required]
    public IFormFile LogoFile { get; set; } = null!;
}

public record TeamMemberDto
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsCaptain { get; set; }
    public bool IsAdmin { get; set; }
    public PlayerPosition Position { get; set; }
    public DateTime JoinedAt { get; set; }
    public string UserPhotoUrl { get; set; }
}

public record TeamMemberDtoForUpdateAdminAndCaptain
{
    public bool IsCaptain { get; set; }
    public bool IsAdmin { get; set; }
}

public class TeamMemberDtoForAdd
{
    public int UserId { get; set; }
    public bool IsCaptain { get; set; }
    public bool IsAdmin { get; set; }
}

public record TeamJoinRequestDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RequestStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
}

public record TeamJoinRequestForCreationDto(int TeamId, int UserId);