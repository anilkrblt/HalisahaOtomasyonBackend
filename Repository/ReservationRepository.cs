using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ReservationRepository
        : RepositoryBase<Reservation>, IReservationRepository
    {
        public ReservationRepository(RepositoryContext ctx) : base(ctx) { }

        /* ---------- Command ---------- */
        public void CreateReservation(Reservation r) => Create(r);
        public void DeleteReservation(Reservation r) => Delete(r);

        /* ---------- Helper (eager-load) --- */
        private static IQueryable<Reservation> IncludeAll(IQueryable<Reservation> q) =>
            q.Include(r => r.Field)
             .ThenInclude(f => f.Facility)
             .Include(r => r.Participants)
                 .ThenInclude(p => p.Team);

        /* ---------- All ------------------- */
        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync(bool trackChanges) =>
            await IncludeAll(FindAll(trackChanges))
                 .OrderByDescending(r => r.SlotStart)
                 .ToListAsync();

        /* ---------- By Id ----------------- */
        public async Task<Reservation?> GetOneReservationAsync(int id, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.Id == id, trackChanges))
                 .SingleOrDefaultAsync();

        /* ---------- By Field -------------- */
        public async Task<IEnumerable<Reservation>> GetReservationsByFieldIdAsync(int fieldId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.FieldId == fieldId, trackChanges))
                 .OrderByDescending(r => r.SlotStart)
                 .ToListAsync();

        /* ---------- By Facility ----------- */
        public async Task<IEnumerable<Reservation>> GetReservationsByFacilityIdAsync(int facilityId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.Field!.FacilityId == facilityId, trackChanges))
                 .OrderByDescending(r => r.SlotStart)
                 .ToListAsync();

        /* ---------- By User (Participant) -- */
        public async Task<IEnumerable<Reservation>> GetReservationsByUserIdAsync(int userId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.Participants.Any(p =>
                                            p.Team.Members.Any(tm => tm.UserId == userId)),
                                    trackChanges))
                 .OrderByDescending(r => r.SlotStart)
                 .ToListAsync();
    }
}
