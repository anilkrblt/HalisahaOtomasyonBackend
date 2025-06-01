using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class MatchRequestRepository
        : RepositoryBase<MatchRequest>, IMatchRequestRepository
    {
        public MatchRequestRepository(RepositoryContext ctx) : base(ctx) { }

        /* ------------- Command ------------- */
        public void CreateMatchRequest(MatchRequest req) => Create(req);
        public void DeleteMatchRequest(MatchRequest req) => Delete(req);

        /* ------------- Helper: eager-load --- */
        private static IQueryable<MatchRequest> IncludeAll(IQueryable<MatchRequest> q) =>
            q.Include(r => r.FromUser)
             .Include(r => r.ToUser)
             .Include(r => r.FromTeam);

        /* ------------- Query: tümü ---------- */
        public async Task<IEnumerable<MatchRequest>> GetAllMatchRequestsAsync(bool trackChanges) =>
            await IncludeAll(FindAll(trackChanges))
                 .OrderByDescending(r => r.CreatedAt)
                 .ToListAsync();

        /* ------------- Query: tek kayıt ----- */
        public async Task<MatchRequest?> GetMatchRequestAsync(int id, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.Id == id, trackChanges))
                 .SingleOrDefaultAsync();

        /* ------------- Gönderen kaptan ------ */
        public async Task<IEnumerable<MatchRequest>> GetRequestsSentByUserAsync(int userId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.FromUserId == userId, trackChanges))
                 .OrderByDescending(r => r.CreatedAt)
                 .ToListAsync();

        /* ------------- Alıcı kaptan --------- */
        public async Task<IEnumerable<MatchRequest>> GetRequestsReceivedByUserAsync(int userId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(r => r.ToUserId == userId, trackChanges))
                 .OrderByDescending(r => r.CreatedAt)
                 .ToListAsync();
    }
}
