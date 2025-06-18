using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RoomParticipantRepository
        : RepositoryBase<RoomParticipant>, IRoomParticipantRepository
    {
        public RoomParticipantRepository(RepositoryContext ctx) : base(ctx) { }

        /* -------- Command -------- */
        public void CreateParticipant(RoomParticipant p) => Create(p);
        public void DeleteParticipant(RoomParticipant p) => Delete(p);



        public async Task<RoomParticipant?> GetByCustomerAsync(int roomId, int customerId)
        {
            return await FindByCondition(p =>
                p.RoomId == roomId && p.CustomerId == customerId, true)
                .FirstOrDefaultAsync();
        }

        /* -------- Tek kayıt -------- */
        public async Task<RoomParticipant?> GetParticipantAsync(
            int roomId, int teamId, bool trackChanges) =>
            await FindByCondition(p => p.RoomId == roomId && p.TeamId == teamId, trackChanges)
                  .Include(p => p.Team)
                  .Include(p => p.Room)
                  .SingleOrDefaultAsync();
        public async Task<RoomParticipant?> GetParticipantByRoomAndUserAsync(int roomId, int userId, bool trackChanges)
        {
            return await RepositoryContext.RoomParticipants
                .Include(rp => rp.Customer)
                .FirstOrDefaultAsync(rp => rp.RoomId == roomId && rp.CustomerId == userId);
        }


        /* -------- Rezervasyona göre liste -------- */
        public async Task<IEnumerable<RoomParticipant>> GetParticipantsByReservationIdAsync(int roomId, bool trackChanges) =>
            await FindByCondition(p => p.RoomId == roomId, trackChanges)
                  .Include(p => p.Team)
                  .OrderByDescending(p => p.IsHome)
                  .ToListAsync();

        public async Task<IEnumerable<RoomParticipant>> GetParticipantsByUserAsync(int userId, bool trackChanges)
        {
            return await FindByCondition(p => p.CustomerId == userId, trackChanges)
                         .Include(p => p.Room)
                         .ThenInclude(r => r.Field)
                         .ToListAsync();
        }


        public async Task<IEnumerable<RoomParticipant>> GetParticipantsByRoomAsync(int roomId, bool trackChanges)
        {
            return await FindByCondition(p => p.RoomId == roomId, trackChanges)
                         .Include(p => p.Customer)
                         .Include(p => p.Team)
                         .ToListAsync();
        }
        /* -------- Takıma göre liste -------- */
        public async Task<IEnumerable<RoomParticipant>> GetParticipantsByTeamIdAsync(int teamId, bool trackChanges) =>
            await FindByCondition(p => p.TeamId == teamId, trackChanges)
                  .Include(p => p.Room)
                      .ThenInclude(r => r.Field)
                  .ToListAsync();
    }




}
