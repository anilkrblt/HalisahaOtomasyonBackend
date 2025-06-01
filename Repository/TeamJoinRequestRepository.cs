using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TeamJoinRequestRepository
        : RepositoryBase<TeamJoinRequest>, ITeamJoinRequestRepository
    {
        public TeamJoinRequestRepository(RepositoryContext ctx) : base(ctx) { }

        /* ---------- Command ---------- */
        public void CreateJoinRequest(TeamJoinRequest r) => Create(r);
        public void DeleteJoinRequest(TeamJoinRequest r) => Delete(r);

        /* ---------- Helper: eager-load -- */
        private static IQueryable<TeamJoinRequest> IncludeAll(IQueryable<TeamJoinRequest> q) =>
            q.Include(r => r.Team)
             .Include(r => r.User);

        /* ---------- Tek kayıt ---------- */
        public async Task<TeamJoinRequest?> GetJoinRequestAsync(int id, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.Id == id, trackChanges))
                 .SingleOrDefaultAsync();

        /* ---------- Takıma göre -------- */
        public async Task<IEnumerable<TeamJoinRequest>> GetRequestsByTeamIdAsync(int teamId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.TeamId == teamId, trackChanges))
                 .OrderByDescending(r => r.CreatedAt)
                 .ToListAsync();

        /* ---------- Oyuncuya göre ------ */
        public async Task<IEnumerable<TeamJoinRequest>> GetRequestsByUserIdAsync(int userId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.UserId == userId, trackChanges))
                 .OrderByDescending(r => r.CreatedAt)
                 .ToListAsync();

        /* ---------- Bekleyenler -------- */
        public async Task<IEnumerable<TeamJoinRequest>> GetPendingRequestsByTeamIdAsync(int teamId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.TeamId == teamId &&
                                         r.Status  == RequestStatus.Pending, trackChanges))
                 .OrderByDescending(r => r.CreatedAt)
                 .ToListAsync();
    }
}
