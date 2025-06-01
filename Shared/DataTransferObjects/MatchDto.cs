// ─────────────────────────────────────────────────────
//  MATCH
// ─────────────────────────────────────────────────────
using Entities.Models;

namespace Shared.DataTransferObjects;

public record MatchDto(
    int      Id,
    int      HomeTeamId,
    int      AwayTeamId,
    int      HomeScore,
    int      AwayScore,
    DateTime DateTime,
    int      FieldId);

public record MatchForCreationDto(
    int      HomeTeamId,
    int      AwayTeamId,
    DateTime DateTime,
    int      FieldId);

public record MatchForUpdateDto(
    int?     HomeScore,
    int?     AwayScore,
    DateTime? DateTime,
    int?     FieldId);

// ─────────────────────────────────────────────────────
//  MATCH REQUEST
// ─────────────────────────────────────────────────────
public record MatchRequestDto(
    int       Id,
    int       FromTeamId,
    int       FromUserId,
    int       ToUserId,
    DateTime  RequestedDateTimeStart,
    DateTime  RequestedDateTimeEnd,
    RequestStatus Status,
    DateTime  CreatedAt,
    DateTime? RespondedAt);

public record MatchRequestForCreationDto(
    int      FromTeamId,
    int      FromUserId,
    int      ToUserId,
    DateTime RequestedDateTimeStart,
    DateTime RequestedDateTimeEnd);

public record MatchRequestRespondDto(RequestStatus Status);
