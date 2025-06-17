using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Team
{
    public int Id { get; set; }

    [MaxLength(128)]
    public string Name { get; set; } = null!;

    [MaxLength(256)]
    public string LogoUrl { get; set; } = string.Empty;

    [MaxLength(128)]
    public string City { get; set; } = string.Empty;

    [MaxLength(128)]
    public string Town { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Content { get; set;   }
    public int MatchPlayed { get; set; }
    public int MatchWon { get; set; }
    public int MatchDrawn { get; set; }
    public int MatchLost { get; set; }

    public ICollection<TeamMember> Members { get; set; } = [];
    public ICollection<TeamJoinRequest> JoinRequests { get; set; } = [];
    public ICollection<TeamComment> Comments { get; set; } = [];
    public ICollection<Match> HomeMatches { get; set; } = [];
    public ICollection<Match> AwayMatches { get; set; } = [];
    public ICollection<RoomParticipant> TeamReservations { get; set; } = [];

    [NotMapped]
    public double AvgRating =>
        Comments.Any()
        ? Comments.Average(c => c.Rating)
        : 0;
}
