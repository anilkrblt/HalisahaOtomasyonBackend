using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RoomRepository : RepositoryBase<Room>, IRoomRepository
    {
        public RoomRepository(RepositoryContext ctx) : base(ctx) { }

        /* ---------- Command ---------- */
        public void CreateRoom(Room r) => Create(r);
        public void DeleteRoom(Room r) => Delete(r);

        /* ---------- Helper (eager-load) --- */
        private static IQueryable<Room> IncludeAll(IQueryable<Room> q) =>
            q.Include(r => r.Field)
             .ThenInclude(f => f.Facility)
             .Include(r => r.Participants)
                 .ThenInclude(p => p.Team);





        /* ---------- All ------------------- */
        public async Task<IEnumerable<Room>> GetAllRoomsAsync(bool trackChanges) =>
            await IncludeAll(FindAll(trackChanges))
                 .OrderByDescending(r => r.SlotStart)
                 .ToListAsync();

        /* ---------- By Id ----------------- */
        public async Task<Room?> GetOneRoomAsync(int id, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.Id == id, trackChanges))
                 .SingleOrDefaultAsync();

        /* ---------- By Field -------------- */
        public async Task<IEnumerable<Room>> GetRoomsByFieldIdAsync(int fieldId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.FieldId == fieldId, trackChanges))
                 .OrderByDescending(r => r.SlotStart)
                 .ToListAsync();

        /* ---------- By Facility ----------- */
        public async Task<IEnumerable<Room>> GetRoomsByFacilityIdAsync(int facilityId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.Field!.FacilityId == facilityId, trackChanges))
                 .OrderByDescending(r => r.SlotStart)
                 .ToListAsync();

        /* ---------- By User (Participant) -- */
        public async Task<IEnumerable<Room>> GetRoomsByUserIdAsync(int userId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.Participants.Any(p =>
                                            p.Team.Members.Any(tm => tm.UserId == userId)),
                                    trackChanges))
                 .OrderByDescending(r => r.SlotStart)
                 .ToListAsync();

        public async Task<IEnumerable<Room>> GetPublicRoomsAsync(RoomAccessType accessType)
        {
            return await RepositoryContext.Rooms
                .AsNoTracking()
                .Where(r => r.AccessType == accessType)
                .Include(r => r.Field)
                .Include(r => r.Participants)
                    .ThenInclude(rp => rp.Team)
                .Include(r => r.Match)
                .ToListAsync();
        }

        public async Task<Room?> GetRoomByCodeAsync(string joinCode, bool trackChanges)
        {
            if (trackChanges)
                return await RepositoryContext.Rooms
                    .FirstOrDefaultAsync(r => r.JoinCode == joinCode);
            else
                return await RepositoryContext.Rooms
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.JoinCode == joinCode);
        }
    }
}
