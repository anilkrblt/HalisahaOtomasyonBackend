using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Shared.DataTransferObjects
{

    public class ReservationDto
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public DateTime SlotStart { get; set; }
        public DateTime SlotEnd { get; set; }
        public decimal PriceTotal { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RoomId { get; set; }
    }

    public class ReservationForCreationDto
    {
        public int FieldId { get; set; }
        public DateTime SlotStart { get; set; }
        public DateTime SlotEnd { get; set; }
        public decimal PriceTotal { get; set; }
        public int RoomId { get; set; }
    }
    public class ReservationPaymentForCreationDto
    {
        public int ReservationId { get; set; }
        public int? RoomParticipantRoomId { get; set; }
        public int? RoomParticipantTeamId { get; set; }
        public decimal Amount { get; set; }
    }
    public class ReservationPaymentDto
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int? RoomParticipantRoomId { get; set; }
        public int? RoomParticipantTeamId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? ProviderRef { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? RefundedAt { get; set; }
    }

}