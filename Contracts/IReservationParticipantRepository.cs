using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IReservationParticipantRepository
    {
        /* Command */
        void CreateParticipant(ReservationParticipant participant);
        void DeleteParticipant(ReservationParticipant participant);

        /* Query – tek satır (bileşik PK) */
        Task<ReservationParticipant?> GetParticipantAsync(int reservationId, int teamId,
                                                          bool trackChanges);

        /* Query – listeler */
        Task<IEnumerable<ReservationParticipant>> GetParticipantsByReservationIdAsync(int reservationId,
                                                                                      bool trackChanges);
        Task<IEnumerable<ReservationParticipant>> GetParticipantsByTeamIdAsync(int teamId,
                                                                               bool trackChanges);

    }
}
