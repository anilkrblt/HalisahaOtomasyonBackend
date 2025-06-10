using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IReservationPaymentRepository
    {
        void CreateReservationPayment(ReservationPayment ReservationPayment);
        void DeleteReservationPayment(ReservationPayment ReservationPayment);
        Task<IEnumerable<ReservationPayment>> GetAllReservationPaymentsAsync(bool trackChanges);
        Task<ReservationPayment?> GetReservationPaymentAsync(int ReservationPaymentId, bool trackChanges);
    }
}