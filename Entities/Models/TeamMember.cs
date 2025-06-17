namespace Entities.Models;

public enum PlayerPosition
{
    GK = 0,
    DF = 1,
    MF = 2,
    FW = 3,
    Utility = 4
}

public class TeamMember
{
    public int TeamId { get; set; }
    public int UserId { get; set; }   
    public Team? Team { get; set; }
    public Customer? User { get; set; }
    public bool IsCaptain { get; set; }
    public bool IsAdmin { get; set; }
    public PlayerPosition Position { get; set; } = PlayerPosition.Utility;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
