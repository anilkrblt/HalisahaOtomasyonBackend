using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public enum FootPreference
{
    Left,
    Right
}
public enum PlayingPosition
{
    Kaleci,
    Defans,
    OrtaSaha,
    SağKanat,
    SolKanat,
    Forvet
}

public class Customer : ApplicationUser
{
    public FootPreference FootPreference { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public int MatchNumber { get; set; }
    public string Positions { get; set; } = string.Empty;

    [NotMapped]
    public double AvgRating =>
    ReceivedComments.Any()
        ? ReceivedComments.Average(c => c.Rating)
        : 0;

    [NotMapped]
    public List<PlayingPosition> PlayingPositions
    {
        get => string.IsNullOrWhiteSpace(Positions)
                ? []
                : Positions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                           .Select(p => Enum.Parse<PlayingPosition>(p))
                           .ToList();

        set => Positions = value is { Count: > 0 }
   ? string.Join(',', value)
   : string.Empty;
    }


    public ICollection<RoomParticipant> RoomParticipations { get; set; } = [];
    public ICollection<UserComment> SentComments { get; set; } = [];
    public ICollection<UserComment> ReceivedComments { get; set; } = [];
    public ICollection<TeamMember> TeamMemberships { get; set; } = [];
    public ICollection<TeamJoinRequest> TeamJoinRequests { get; set; } = [];
    public ICollection<Comment> AuthoredComments { get; set; } = [];
    public ICollection<Friendship> Friends1 { get; set; } = [];
    public ICollection<Friendship> Friends2 { get; set; } = [];

    [NotMapped]
    public IEnumerable<Customer> AllFriends =>
        Friends1.Where(f => f.Status == FriendshipStatus.Accepted).Select(f => f.User2!)
        .Concat(Friends2.Where(f => f.Status == FriendshipStatus.Accepted).Select(f => f.User1!))
        .Distinct();
    public ICollection<Room> Rooms { get; set; } = [];
    public ICollection<MonthlyMembership> MonthlyMemberships { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
}
