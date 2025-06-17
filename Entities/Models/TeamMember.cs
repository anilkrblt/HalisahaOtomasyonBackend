using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class TeamMember
{
    public int TeamId { get; set; }
    public int UserId { get; set; }

    public Team? Team { get; set; }
    public Customer? User { get; set; }

    public bool IsCaptain { get; set; }
    public bool IsAdmin { get; set; }

    public string Position { get; set; } = string.Empty;

    [NotMapped]
    public List<string> Positions
    {
        get => PositionHelper.ParsePositions(Position);
        set => Position = value is { Count: > 0 }
            ? string.Join(",", value)
            : string.Empty;
    }

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
