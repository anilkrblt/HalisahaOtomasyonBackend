using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class ReservationNotFoundException : NotFoundException
    {
        public ReservationNotFoundException(int reservationId)
         : base($"Reservation with id: {reservationId} doesn't exist in the database.")
        {
        }
    }
}