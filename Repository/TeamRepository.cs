using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TeamRepository : RepositoryBase<Team>, ITeamRepository
    {
        public TeamRepository(RepositoryContext ctx) : base(ctx) { }

        /* ---------- Command ---------- */
        public void CreateTeam(Team team) => Create(team);
        public void DeleteTeam(Team team) => Delete(team);

        /* ---------- Helper: eager load -- */
        private static IQueryable<Team> IncludeAll(IQueryable<Team> q) =>
            q.Include(t => t.Members).ThenInclude(m => m.User)
             .Include(t => t.Comments)
             .Include(t => t.HomeMatches)
             .Include(t => t.AwayMatches);

        /* ---------- Query: tümü --------- */
        public async Task<IEnumerable<Team>> GetAllTeamsAsync(bool trackChanges) =>
            await IncludeAll(FindAll(trackChanges))
                    .OrderBy(t => t.Name)
                    .ToListAsync();

        /* ---------- Query: tek kayıt ----- */
        public async Task<Team?> GetTeamAsync(int teamId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(t => t.Id == teamId, trackChanges))
                 .SingleOrDefaultAsync();

        /* ---------- Query: şehir bazlı --- */
        public async Task<IEnumerable<Team>> GetTeamsByCityAsync(string city, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(t => t.City == city, trackChanges))
                 .OrderBy(t => t.Name)
                 .ToListAsync();

        /* ---------- Query: ada göre arama */
        public async Task<IEnumerable<Team>> SearchTeamsByNameAsync(string keyword, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(t => t.Name.Contains(keyword), trackChanges))
                 .OrderBy(t => t.Name)
                 .ToListAsync();
    }
}
