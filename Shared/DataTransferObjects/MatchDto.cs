// Shared.DataTransferObjects/MatchDtos.cs
namespace Shared.DataTransferObjects;

/*──────────────── MATCH (görüntü / skor) ───────────*/

public record MatchDto(
    int Id,
    int? HomeTeamId,
    int? AwayTeamId,
    int HomeScore,
    int AwayScore,
    DateTime StartTime,       // <— eski DateTime alanı
    int? FieldId,         // <— opsiyonel hale getirildi
    int ReservationId);

/*──── Skor güncelleme — controller’da POST /matches/{id}/score ────*/
public record ScoreUpdateDto(
    int Home,
    int Away);
