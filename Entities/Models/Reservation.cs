using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum ReservationStatus
    {
        PendingPayment,   // ödeme bekleniyor
        Confirmed,        // ödeme tamam → saha kesinleşti
        Cancelled,        // iptal
        Expired,          // ödeme süresi geçti
        Played            // maç oynandı
    }

    public class Reservation
    {
        public int Id { get; set; }

        /* Saha & zaman */
        public int FieldId { get; set; }
        public Field Field { get; set; } = null!;

        public DateTime SlotStart { get; set; }
        public DateTime SlotEnd { get; set; }   // 1 saat değilse dışarıdan set edilsin

        /* Ödeme & durum */
        public decimal PriceTotal { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.PendingPayment;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /* —— İlişkiler —— */
        public int RoomId { get; set; }           // 1-1
        public Room Room { get; set; } = null!;

        public ICollection<ReservationPayment> Payments { get; set; } = [];
    }
}