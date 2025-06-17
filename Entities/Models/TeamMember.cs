namespace Entities.Models;

public class TeamMember
{
    /* --------- Composite PK --------- */
    public int TeamId { get; set; }
    public int UserId { get; set; }          // CustomerId

    /* --------- Navigation --------- */
    public Team? Team { get; set; }
    public Customer? User { get; set; }

    /* --------- Özellikler --------- */
    public bool IsCaptain { get; set; }
    public bool IsAdmin { get; set; }
    public string Position { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
public static class PositionHelper
{
    public static List<string> ParsePositions(string? positionStr)
    {
        if (string.IsNullOrWhiteSpace(positionStr))
            return new List<string>();

        return positionStr
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .ToList();
    }
}
