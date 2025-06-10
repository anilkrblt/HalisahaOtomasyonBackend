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
        await FindAll(trackChanges).ToListAsync();

        public async Task<ReservationPayment?> GetReservationPaymentAsync(int ReservationPaymentId, bool trackChanges) =>
        await FindByCondition(rp => ReservationPaymentId.Equals(rp.Id), trackChanges).FirstOrDefaultAsync();
    }
}