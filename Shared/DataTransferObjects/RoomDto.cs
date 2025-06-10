// Shared.DataTransferObjects/RoomDtos.cs
using System.Text.Json.Serialization;
using Entities.Models;

namespace Shared.DataTransferObjects;

/*──────────────── ROOM ─────────────────────────────*/

/* — API’den dönen — */
public record RoomDto(
    int RoomId,          // <— 1) isim değişti
    int FieldId,
    DateTime SlotStart,
    DateTime SlotEnd,
    RoomAccessType AccessType,
    string? JoinCode,
    int MaxPlayers,
    decimal PricePerPlayer,
    RoomStatus Status,
    DateTime CreatedAt,
    MatchDto Match);

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

public record RoomParticipantDto(
    int RoomId,
    int TeamId,
    bool IsHome,
    ParticipantStatus Status,
    bool HasPaid,
    decimal? PaidAmount,
    DateTime? RespondedAt);

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
