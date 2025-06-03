namespace Entities.Models;

public class Match
{
    public int Id { get; set; }

    /* 1-1 ilişki */
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    /* Takımlar – maç onaylandığında atanır */
    public int? HomeTeamId { get; set; }
    public Team? HomeTeam { get; set; }
    public int? AwayTeamId { get; set; }
    public Team? AwayTeam { get; set; }

    /* Skor */
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    /* Başlangıç zamanı – genelde Reservation.SlotStart kopyası */
    public DateTime StartTime { get; set; }

    /* Kolaylık için saha FK (rapor/sorgu) */
    public int FieldId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
