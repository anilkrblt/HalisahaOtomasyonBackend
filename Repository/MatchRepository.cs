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

        /* -------- Helpers for eager-loading -------- */
        private static IQueryable<Match> IncludeAll(IQueryable<Match> q) =>
            q.Include(m => m.Room)
             .ThenInclude(r => r.Field)
             .ThenInclude(f => f.Facility)
             .Include(m => m.Room)
             .ThenInclude(r => r.Participants);

        /* -------- Query: All matches -------- */
        public async Task<IEnumerable<Match>> GetAllMatchesAsync(bool trackChanges) =>
            await IncludeAll(FindAll(trackChanges))
                .OrderByDescending(m => m.Room.SlotStart)
                .ToListAsync();

        /* -------- Query: Single by Id -------- */
        public async Task<Match?> GetMatchAsync(int matchId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(m => m.Id == matchId, trackChanges))
                .SingleOrDefaultAsync();

        /* -------- Query: By FieldId -------- */
        public async Task<IEnumerable<Match>> GetMatchesByFieldIdAsync(int fieldId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(m => m.Room.FieldId == fieldId, trackChanges))
                .OrderByDescending(m => m.Room.SlotStart)
                .ToListAsync();

        /* -------- Query: By TeamId (home or away) -------- */
        public async Task<IEnumerable<Match>> GetMatchesByTeamIdAsync(int teamId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId, trackChanges))
                .OrderByDescending(m => m.Room.SlotStart)
                .ToListAsync();
    }
}
