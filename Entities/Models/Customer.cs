using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public enum FootPreference { Left, Right }
public enum PlayingPosition { GK, DF, MF, FW, Utility }

public class Customer : ApplicationUser
{
        /* --------- Özel Alanlar --------- */
        public FootPreference FootPreference { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public int MatchNumber { get; set; }

        /* Çoklu pozisyonu CSV olarak saklıyoruz */
        public string Positions { get; set; } = string.Empty;

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

        /* --------- Navigation’lar --------- */

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


        public ICollection<Reservation> Reservations { get; set; } = [];
        public ICollection<MonthlyMembership> MonthlyMemberships { get; set; } = [];
        public ICollection<Notification> Notifications { get; set; } = [];
}
