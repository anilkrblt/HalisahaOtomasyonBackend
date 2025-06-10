// Shared.DataTransferObjects/RoomDtos.cs
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
public record RoomCreateDto(
    int FieldId,
    DateTime SlotStart,
    RoomAccessType AccessType,
    int MaxPlayers);

/*──────────────── PARTICIPANT ─────────────────────*/

public record RoomParticipantDto(        // <— 2) sınıf adı değişti
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
