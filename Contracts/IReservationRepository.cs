using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IReservationRepository
    {
        void CreateReservation(Reservation reservation);
        void DeleteReservation(Reservation reservation);

        Task<IEnumerable<Reservation>> GetAllReservationsAsync(bool trackChanges);
        Task<Reservation?> GetOneReservationAsync(int id, bool trackChanges);

        Task<IEnumerable<Reservation>> GetReservationsByFieldIdAsync(int fieldId, bool trackChanges);
        Task<IEnumerable<Reservation>> GetReservationsByFacilityIdAsync(int facilityId, bool trackChanges);
        Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId, bool trackChanges); // katılımcı
    }

}