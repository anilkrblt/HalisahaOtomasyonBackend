namespace Entities.Models;

public class Match
{
    public int Id { get; set; }

    /* ------ Takımlar ------ */
    public int HomeTeamId { get; set; }
    public Team? HomeTeam { get; set; }

    public int AwayTeamId { get; set; }
    public Team? AwayTeam { get; set; }

    public int HomeScore { get; set; }
    public int AwayScore { get; set; }

    /* ------ Zaman & Saha ------ */
    public DateTime DateTime { get; set; }             // Maç başlangıcı

    public int FieldId  { get; set; }                  // Yeni: saha FK
    public Field? Field { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
