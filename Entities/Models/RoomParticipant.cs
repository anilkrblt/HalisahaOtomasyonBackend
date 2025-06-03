namespace Entities.Models;

public class RoomParticipant
{
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public bool IsHome { get; set; }
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Invited;

    /* Ödeme */
    public bool HasPaid { get; set; }
    public decimal? PaidAmount { get; set; }
}
