using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ReservationPayment
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; } = null!;

        public int? RoomParticipantRoomId { get; set; }   // kimin ödediği
        public int? RoomParticipantTeamId { get; set; }
        public RoomParticipant? Participant { get; set; }

        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string? ProviderRef { get; set; }    // ödeme linki / intent id
        public DateTime? PaidAt { get; set; }
        public DateTime? RefundedAt { get; set; }
    }
}