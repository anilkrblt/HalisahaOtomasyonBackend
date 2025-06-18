using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IReservationRepository
    {
        void CreateReservation(Reservation Reservation);
        void DeleteReservation(Reservation Reservation);
        Task<IEnumerable<Reservation>> GetAllReservationsAsync(bool trackChanges);
        Task<Reservation?> GetReservationAsync(int ReservationId, bool trackChanges);
        /// <summary>
        /// Belirli saha + başlangıç saatine çakışan tüm rezervasyonları getirir
        /// </summary>
        Task<IEnumerable<Reservation>> GetByFieldAsync(int fieldId, DateTime slotStart);

        Task<IEnumerable<Reservation>> GetReservationsWithPaymentsByOwnerAsync(int ownerId);

    }
}