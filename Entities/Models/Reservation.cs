using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    public enum ReservationStatus   // Slot’ın kaderi
    {
        PendingOpponent,   // Rakip arıyor
        WaitingConfirm,    // Rakip daveti kabul etmedi-reddetmedi
        Confirmed,         // Her iki taraf onayladı
        Cancelled,
        Expired,
        Played             // Maç yapıldı
    }

    public enum ParticipantStatus   // Takım bazında
    {
        Invited,
        Accepted,
        Rejected,
        Cancelled
    }



    public class Reservation
    {
        public int Id { get; set; }

        public int FieldId { get; set; }
        public Field Field { get; set; }

        public DateTime SlotStart { get; set; }          // örn. 2025-05-20 18:00
        public DateTime SlotEnd => SlotStart.AddHours(1);

        public ReservationStatus Status { get; set; } = ReservationStatus.PendingOpponent;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /* Katılımcılar (Team-ReservationParticipant sistemi) aynı kalıyor */
        public ICollection<ReservationParticipant> Participants { get; set; } = [];
    }

}