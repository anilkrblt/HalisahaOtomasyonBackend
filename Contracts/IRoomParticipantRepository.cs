using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRoomParticipantRepository
    {
        /* Command */
        void CreateParticipant(RoomParticipant participant);
        void DeleteParticipant(RoomParticipant participant);

        /* Query – tek satır (bileşik PK) */
        Task<RoomParticipant?> GetParticipantAsync(int reservationId, int teamId, bool trackChanges);

        /* Query – listeler */
        Task<IEnumerable<RoomParticipant>> GetParticipantsByReservationIdAsync(int reservationId, bool trackChanges);
        Task<IEnumerable<RoomParticipant>> GetParticipantsByTeamIdAsync(int teamId, bool trackChanges);

    }
}
