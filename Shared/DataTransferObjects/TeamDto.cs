// ─────────────────────────────────────────────────────────────
// 1) TAKIM (Team)
// ─────────────────────────────────────────────────────────────

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
    double AvgRating,
    IEnumerable<TeamMemberDto> Members);   // ← eklendi


public record TeamForCreationDto(
    string Name,
    string City,
    string Town);

public record TeamForUpdateDto(
    string Name,
    string City,
    string Town);
// Shared/DataTransferObjects/TeamDtos.cs
public class TeamLogoUploadDto
{
    [Required]
    public IFormFile LogoFile { get; set; } = null!;
}


// ─────────────────────────────────────────────────────────────
// 2) TAKIM ÜYESİ (TeamMember)
// ─────────────────────────────────────────────────────────────

public record TeamMemberDto(
    int UserId,
    bool IsCaptain,
    PlayerPosition Position,
    DateTime JoinedAt);

/// <summary>
/// Üye ekleme için kullanılır. TeamId ve UserId route/body’de gelebileceği için
/// DTO sadece opsiyonel alanları içerir.
/// </summary>

public class TeamMemberForAddDto
{
    public int UserId { get; set; }
    public bool IsCaptain { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PlayerPosition Position { get; set; }
}



// ─────────────────────────────────────────────────────────────
// 3) TAKIMA KATILIM İSTEĞİ (TeamJoinRequest)
// ─────────────────────────────────────────────────────────────

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


public record TeamJoinRequestForCreationDto(
    int TeamId,
    int UserId);
