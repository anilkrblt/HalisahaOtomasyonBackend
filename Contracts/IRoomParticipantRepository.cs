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
        Task<RoomParticipant?> GetByCustomerAsync(int roomId, int customerId);
        Task<RoomParticipant?> GetByKeyAsync(int roomId, int teamId);

        Task<IEnumerable<RoomParticipant>> GetParticipantsByUserAsync(int userId, bool trackChanges);


        Task<IEnumerable<RoomParticipant>> GetParticipantsByRoomAsync(int roomId, bool trackChanges);
        Task<RoomParticipant?> GetParticipantByRoomAndUserAsync(int roomId, int userId, bool trackChanges);

        /* Query – tek satır (bileşik PK) */
        Task<RoomParticipant?> GetParticipantAsync(int reservationId, int teamId, bool trackChanges);

        /* Query – listeler */
        Task<IEnumerable<RoomParticipant>> GetParticipantsByReservationIdAsync(int reservationId, bool trackChanges);
        Task<IEnumerable<RoomParticipant>> GetParticipantsByTeamIdAsync(int teamId, bool trackChanges);

    }
}
