using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ReservationParticipantRepository
        : RepositoryBase<ReservationParticipant>, IReservationParticipantRepository
    {
        public ReservationParticipantRepository(RepositoryContext ctx) : base(ctx) { }

        /* -------- Command -------- */
        public void CreateParticipant(ReservationParticipant p) => Create(p);
        public void DeleteParticipant(ReservationParticipant p) => Delete(p);

        /* -------- Tek kayıt -------- */
        public async Task<ReservationParticipant?> GetParticipantAsync(
            int reservationId, int teamId, bool trackChanges) =>
            await FindByCondition(p => p.ReservationId == reservationId &&
                                       p.TeamId        == teamId, trackChanges)
                  .Include(p => p.Team)
                  .Include(p => p.Reservation)
                  .SingleOrDefaultAsync();

        /* -------- Rezervasyona göre liste -------- */
        public async Task<IEnumerable<ReservationParticipant>> GetParticipantsByReservationIdAsync(
            int reservationId, bool trackChanges) =>
            await FindByCondition(p => p.ReservationId == reservationId, trackChanges)
                  .Include(p => p.Team)
                  .OrderByDescending(p => p.IsHome)
                  .ToListAsync();

        /* -------- Takıma göre liste -------- */
        public async Task<IEnumerable<ReservationParticipant>> GetParticipantsByTeamIdAsync(
            int teamId, bool trackChanges) =>
            await FindByCondition(p => p.TeamId == teamId, trackChanges)
                  .Include(p => p.Reservation)
                      .ThenInclude(r => r.Field)
                  .OrderByDescending(p => p.RespondedAt)
                  .ToListAsync();
    }
}
