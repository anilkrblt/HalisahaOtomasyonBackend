using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ReservationPaymentRepository : RepositoryBase<ReservationPayment>, IReservationPaymentRepository
    {
        public ReservationPaymentRepository(RepositoryContext context) : base(context)
        {

        }

        public void CreateReservationPayment(ReservationPayment ReservationPayment) => Create(ReservationPayment);

        public void DeleteReservationPayment(ReservationPayment ReservationPayment) => Delete(ReservationPayment);

        public async Task<IEnumerable<ReservationPayment>> GetAllReservationPaymentsAsync(bool trackChanges) =>
        await RepositoryContext.ReservationPayments
            .Include(p => p.Participant)
                .ThenInclude(pa => pa.Customer)
            .ToListAsync();


        public async Task<ReservationPayment?> GetReservationPaymentAsync(int id, bool trackChanges) =>
    await RepositoryContext.ReservationPayments
        .Include(p => p.Participant)
            .ThenInclude(pa => pa.Customer)
        .FirstOrDefaultAsync(p => p.Id == id);

    }
}