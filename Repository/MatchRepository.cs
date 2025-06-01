using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class MatchRepository : RepositoryBase<Match>, IMatchRepository
    {
        public MatchRepository(RepositoryContext ctx) : base(ctx) { }

        /* -------- Command -------- */
        public void CreateMatch(Match match) => Create(match);
        public void DeleteMatch(Match match) => Delete(match);

        /* -------- Helpers for eager-load -------- */
        private static IQueryable<Match> IncludeAll(IQueryable<Match> q) =>
            q.Include(m => m.HomeTeam)
             .Include(m => m.AwayTeam)
             .Include(m => m.Field)
                 .ThenInclude(f => f.Facility);   // tesis bilgisi de lazım olursa

        /* -------- Query: Hepsi -------- */
        public async Task<IEnumerable<Match>> GetAllMatchesAsync(bool trackChanges) =>
            await IncludeAll(FindAll(trackChanges))
                 .OrderByDescending(m => m.DateTime)
                 .ToListAsync();

        /* -------- Query: Tek kayıt -------- */
        public async Task<Match?> GetMatchAsync(int matchId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(m => m.Id == matchId, trackChanges))
                 .SingleOrDefaultAsync();

        /* -------- Query: Saha bazlı -------- */
        public async Task<IEnumerable<Match>> GetMatchesByFieldIdAsync(int fieldId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(m => m.FieldId == fieldId, trackChanges))
                 .OrderByDescending(m => m.DateTime)
                 .ToListAsync();

        /* -------- Query: Takım bazlı (home veya away) -------- */
        public async Task<IEnumerable<Match>> GetMatchesByTeamIdAsync(int teamId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId, trackChanges))
                 .OrderByDescending(m => m.DateTime)
                 .ToListAsync();
    }
}
