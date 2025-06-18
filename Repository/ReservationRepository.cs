using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ReservationRepository : RepositoryBase<Reservation>, IReservationRepository
    {
        public ReservationRepository(RepositoryContext context) : base(context)
        {

        }

        public void CreateReservation(Reservation Reservation) => Create(Reservation);

        public void DeleteReservation(Reservation Reservation) => Delete(Reservation);

        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync(bool trackChanges) =>
        await FindAll(trackChanges).ToListAsync();

        public async Task<Reservation?> GetReservationAsync(int ReservationId, bool trackChanges) =>
        await FindByCondition(r => r.Id == ReservationId, trackChanges).FirstOrDefaultAsync();


        /// <summary>
        /// Aynı sahada slotStart–slotStart+1h aralığına denk gelen (iptal/expired
        /// olmayan) rezervasyonları döndürür.
        /// </summary>
        public async Task<IEnumerable<Reservation>> GetByFieldAsync(int fieldId, DateTime slotStart)
        {
            var slotEnd = slotStart.AddHours(1);

            return await FindByCondition(r =>
                       r.FieldId == fieldId &&
                       r.SlotStart < slotEnd &&          // rezervasyon slotu, istenen aralığa dokunuyor mu?
                       r.SlotEnd > slotStart &&
                       r.Status != ReservationStatus.Cancelled &&
                       r.Status != ReservationStatus.Expired,
                       trackChanges: false)
                   .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetReservationsWithPaymentsByOwnerAsync(int ownerId)
        {
            return await RepositoryContext.Reservations
                .Include(r => r.Payments)
                    .ThenInclude(p => p.Participant)
                        .ThenInclude(pa => pa.Customer)
                .Include(r => r.Field)
                    .ThenInclude(f => f.Facility) // 👈 Facility dahil edilmeli
                .Where(r => r.Field.Facility.OwnerId == ownerId) // 👈 Doğru erişim
                .ToListAsync();
        }


    }
}