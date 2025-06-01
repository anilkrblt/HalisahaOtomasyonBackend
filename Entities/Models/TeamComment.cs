namespace Entities.Models;

public class TeamComment
{
    public int Id { get; set; }

    /* --------- Yorumu Yazan Oyuncu --------- */
    public int FromUserId { get; set; }
    public Customer? FromUser { get; set; }

    /* --------- Hedef Takım --------- */
    public int ToTeamId { get; set; }
    public Team? ToTeam { get; set; }

    /* --------- İçerik --------- */
    public string Content { get; set; } = string.Empty;

    /// <summary>0–5 arası yıldız puan</summary>
    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    /* --------- Opsiyonel soft-delete --------- */
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
