using System.ComponentModel.DataAnnotations;

namespace Entities.Models;

public enum RoomStatus
{
    PendingOpponent,
    WaitingConfirm,
    Confirmed,
    Cancelled,
    Expired,
    Played
}

public enum ParticipantStatus { Invited, Accepted, Rejected, Cancelled }
public enum RoomAccessType { Public, Private }

public class Room
{
    public int Id { get; set; }

    /* Saha & saat */
    public int FieldId { get; set; }
    public Field Field { get; set; } = null!;

    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd => SlotStart.AddHours(1);

    /* Oda ayarları */
    public RoomAccessType AccessType { get; set; } = RoomAccessType.Public;
    [MaxLength(10)]
    public string? JoinCode { get; set; }
    public int MaxPlayers { get; set; } = 10;
    public decimal PricePerPlayer { get; set; }

    public RoomStatus Status { get; set; } = RoomStatus.PendingOpponent;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /* 1-1 Match */
    public Match? Match { get; set; }
    public Reservation? Reservation { get; set; }


    public ICollection<RoomParticipant> Participants { get; set; } = [];
}
