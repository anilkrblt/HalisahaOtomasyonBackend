using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface IReservationService
    {
        // Rezervasyonlar
        Task<IEnumerable<ReservationDto>> GetAllReservationsAsync(bool trackChanges = false);
        Task<ReservationDto> GetReservationByIdAsync(int reservationId, bool trackChanges = false);
        Task<ReservationDto> CreateReservationAsync(ReservationForCreationDto dto);
        Task DeleteReservationAsync(int reservationId);
        Task<IEnumerable<ReservationDto>> GetOverlappingReservationsByFieldAsync(int fieldId, DateTime slotStart);

        // Ödemeler
        Task<IEnumerable<ReservationPaymentDto>> GetAllReservationPaymentsAsync(bool trackChanges = false);
        Task<ReservationPaymentDto> GetReservationPaymentByIdAsync(int id, bool trackChanges = false);
        Task<ReservationPaymentDto> CreateReservationPaymentAsync(ReservationPaymentForCreationDto dto);
        Task DeleteReservationPaymentAsync(int id);
    }

}