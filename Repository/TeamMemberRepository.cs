using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TeamMemberRepository
        : RepositoryBase<TeamMember>, ITeamMemberRepository
    {
        public TeamMemberRepository(RepositoryContext ctx) : base(ctx) { }

        /* -------- Command -------- */
        public void AddMember(TeamMember member)    => Create(member);
        public void RemoveMember(TeamMember member) => Delete(member);

        /* -------- Helper: eager-load --- */
        private static IQueryable<TeamMember> IncludeAll(IQueryable<TeamMember> q) =>
            q.Include(tm => tm.Team)
             .Include(tm => tm.User);

        /* -------- Tek kayıt -------- */
        public async Task<TeamMember?> GetMemberAsync(int teamId, int userId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(m => m.TeamId == teamId && m.UserId == userId, trackChanges))
                 .SingleOrDefaultAsync();

        /* -------- Takıma göre liste ---- */
        public async Task<IEnumerable<TeamMember>> GetMembersByTeamIdAsync(int teamId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(m => m.TeamId == teamId, trackChanges))
                 .OrderByDescending(m => m.IsCaptain)   // kaptan(lar) üstte
                 .ThenBy(m => m.JoinedAt)
                 .ToListAsync();

        /* -------- Oyuncuya göre takımlar - */
        public async Task<IEnumerable<TeamMember>> GetTeamsByUserIdAsync(int userId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(m => m.UserId == userId, trackChanges))
                 .OrderByDescending(m => m.JoinedAt)
                 .ToListAsync();
    }
}
