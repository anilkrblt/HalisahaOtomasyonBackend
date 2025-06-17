using System.Text.Json.Serialization;
using Entities.Models;

namespace Shared.DataTransferObjects;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? LogoUrl { get; set; }
    public string City { get; set; }
    public string Town { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MatchPlayed { get; set; }
    public int MatchWon { get; set; }
    public int MatchDrawn { get; set; }
    public int MatchLost { get; set; }
    public string Content { get; set; }
    public double AvgRating { get; set; }
    public IEnumerable<TeamMemberDto> Members { get; set; }
}

public class TeamForCreationDto
{
    public string Name { get; set; }
    public string City { get; set; }
    public string Town { get; set; }
    public string Content { get; set; }
    public string? LogoUrl { get; set; }
}

public class TeamForUpdateDto
{
    public string Name { get; set; }
    public string City { get; set; }
    public string Town { get; set; }
    public string Content { get; set; }
    public string? LogoUrl {  get; set; }
}

public class TeamMemberDto
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsCaptain { get; set; }
    public double AvgRating { get; set; }
    public bool IsAdmin { get; set; }
    public PlayerPosition Position { get; set; }
    public DateTime JoinedAt { get; set; }
    public string UserPhotoUrl { get; set; }
}

public class TeamMemberDtoForUpdateAdminAndCaptain
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

public class TeamJoinRequestDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RequestStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } 
    public DateTime? RespondedAt { get; set; }
}

public class TeamJoinRequestDtoForUpdate
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RequestStatus Status { get; set; }
}