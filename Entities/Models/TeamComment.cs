namespace Entities.Models;

public class TeamComment
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public Customer? FromUser { get; set; }
    public int ToTeamId { get; set; }
    public Team? ToTeam { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
