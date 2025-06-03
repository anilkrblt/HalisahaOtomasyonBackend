using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class RoomNotFoundException : NotFoundException
    {
        public RoomNotFoundException(int reservationId)
         : base($"Room with id: {reservationId} doesn't exist in the database.")
        {
        }
    }
    public class ParticipantNotFoundException : NotFoundException
    {
        public ParticipantNotFoundException(int roomId, int teamId)
         : base($"Room with id: {roomId} or   team with id: {teamId} doesn't exist in the database.")
        {
        }
    }
}