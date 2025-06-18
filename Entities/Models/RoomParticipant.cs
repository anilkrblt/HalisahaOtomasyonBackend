namespace Entities.Models;

public enum PaymentStatus
{
    Unpaid = 0,     // Henüz ödeme seçilmedi
    Pending = 1,    // Ödeme linki alındı, işlem devam ediyor
    Paid = 2,       // Ödeme başarılı
    Refunded = 3    // Tam/parsiyel iade
}


public class RoomParticipant
{
    public int Id { get; set; } // EF için yeni PK

    /* --------- Composite PK --------- */
    public int RoomId { get; set; }
    public Room Room { get; set; } = null!;

    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    public string? ChargeId { get; set; }  // Stripe Charge ID (refund için kullanılır)



    public int? CustomerId { get; set; }           // bireysel oyuncu
    public Customer? Customer { get; set; }

    /* --------- Maç ayarı --------- */
    public bool IsHome { get; set; }
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Invited;

    /* --------- Hazır & Ödeme --------- */
    public bool IsReady { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    public decimal? PaidAmount { get; set; }
}
