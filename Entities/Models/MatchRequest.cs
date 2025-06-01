namespace Entities.Models;

/// <summary>
/// Takım kaptanından başka bir takım kaptanına maç daveti.
/// </summary>
public class MatchRequest
{
    public int Id { get; set; }

    /* ---- İstenen zaman aralığı (1 saatlik slotlar seçiyorsan End = Start.AddHours(1)) ---- */
    public DateTime RequestedDateTimeStart { get; set; }
    public DateTime RequestedDateTimeEnd   { get; set; }

    public RequestStatus Status   { get; set; } = RequestStatus.Pending;
    public DateTime CreatedAt     { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt  { get; set; }

    /* ---- Gönderen kaptan & takımı ---- */
    public int FromUserId  { get; set; }
    public Customer? FromUser { get; set; }

    public int FromTeamId  { get; set; }
    public Team? FromTeam  { get; set; }

    /* ---- Alıcı kaptan ---- */
    public int ToUserId    { get; set; }
    public Customer? ToUser { get; set; }
}
