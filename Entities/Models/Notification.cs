namespace Entities.Models;

public enum NotificationType
{
    Comment,        // Bir yoruma dair bildirim
    Reservation,    // Rezervasyon onay/iptal vb.
    System,         // Genel sistem bildirimi
    Warning,
    Info    // Kural ihlali, uyarı vs.
}

public class Notification
{
    public int Id { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;

    /* --------- Bildirim Hedefi --------- */
    public int? UserId { get; set; }          // CustomerId
    public Customer? User { get; set; }

    public int? FacilityId { get; set; }
    public Facility? Facility { get; set; }

    /* İlgili varlık linki (opsiyonel) */
    public string? EntityType { get; set; }   // "Comment", "Reservation" vs.
    public int? EntityId { get; set; }

    public NotificationType Type { get; set; } = NotificationType.System;
}
