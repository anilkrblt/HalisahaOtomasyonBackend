// Shared.DataTransferObjects/RoomDtos.cs
using System.Text.Json.Serialization;
using Entities.Models;

namespace Shared.DataTransferObjects;

/*──────────────── ROOM ─────────────────────────────*/

/* — API’den dönen — */

public record RoomDto
{
    public int RoomId { get; set; }
    public int FieldId { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public RoomAccessType AccessType { get; set; }
    public string? JoinCode { get; set; }
    public int MaxPlayers { get; set; }
    public decimal PricePerPlayer { get; set; }
    public RoomStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public MatchDto? Match { get; set; }
}


/* — Oluştururken gönderilen — */
public record RoomCreateDto
{
    public int FieldId { get; set; }
    public DateTime SlotStart { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public RoomAccessType AccessType { get; set; }
    public int MaxPlayers { get; set; }
}

/*──────────────── PARTICIPANT ─────────────────────*/

public record RoomParticipantDto
{
    public int RoomId { get; set; }
    public int TeamId { get; set; }
    public bool IsHome { get; set; }
    public ParticipantStatus Status { get; set; }
    public bool HasPaid { get; set; }
    public decimal? PaidAmount { get; set; }
    public DateTime? RespondedAt { get; set; }
}


public record RoomParticipantForCreationDto(
    int RoomId,
    int TeamId,
    bool IsHome);

/*──────────────── PAYMENT ─────────────────────────*/

public record PaymentDto(
    decimal Amount,
    string ProviderToken);

/*──────────────── MONTHLY MEMBERSHIP ──────────────*/

public record MonthlyMembershipDto(
    int Id,
    int FieldId,
    int UserId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime ExpirationDate);

public record MonthlyMembershipForCreationDto(
    int FieldId,
    int UserId,
    DateTime ExpirationDate);

public record MonthlyMembershipForUpdateDto(
    DateTime? ExpirationDate);
