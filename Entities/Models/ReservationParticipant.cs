using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
public class ReservationParticipant
{
    public int ReservationId { get; set; }
    public Reservation Reservation { get; set; }

    public int TeamId { get; set; }
    public Team Team { get; set; }

    public bool IsHome { get; set; }   
    public ParticipantStatus Status { get; set; } = ParticipantStatus.Invited;

    public DateTime? RespondedAt { get; set; }
}
}