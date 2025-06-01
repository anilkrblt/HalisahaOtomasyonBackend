using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public enum FriendshipStatus
{
    Pending  = 0,   // İstek gönderildi
    Accepted = 1,   // Karşılıklı arkadaş
    Rejected = 2,   // Reddedildi
    Blocked  = 3    // Engellendi
}


public class Friendship
{
    public int UserId1 { get; set; }          
    public int UserId2 { get; set; }          

    public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(UserId1))]
    public Customer? User1 { get; set; }

    [ForeignKey(nameof(UserId2))]
    public Customer? User2 { get; set; }
}
