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

        public void AddMember(TeamMember member) => Create(member);
        public void RemoveMember(TeamMember member) => Delete(member);

        private static IQueryable<TeamMember> IncludeAll(IQueryable<TeamMember> q) =>
            q.Include(tm => tm.Team)
             .Include(tm => tm.User);

        private static IQueryable<TeamMember> IncludeUser(IQueryable<TeamMember> q) =>
            q.Include(tm => tm.User);

        public async Task<TeamMember?> GetMemberAsync(int teamId, int userId, bool trackChanges) =>
            await IncludeAll(
                    FindByCondition(m => m.TeamId == teamId && m.UserId == userId, trackChanges))
                 .SingleOrDefaultAsync();

        public async Task<TeamMember?> GetTeamMemberAsync(int teamId, int userId, bool trackChanges) =>
            await IncludeUser(
                FindByCondition(m => m.TeamId == teamId && m.UserId == userId, trackChanges))
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<TeamMember>> GetMembersByTeamIdWithUserAsync(int teamId, bool trackChanges) =>
            await IncludeUser(
                    FindByCondition(m => m.TeamId == teamId, trackChanges))
                 .OrderByDescending(m => m.IsCaptain)
                 .ThenBy(m => m.JoinedAt)
                 .ToListAsync();

        public async Task<IEnumerable<TeamMember>> GetMembersByTeamIdAsync(int teamId, bool trackChanges) =>
            await FindByCondition(m => m.TeamId == teamId, trackChanges)
                .OrderByDescending(m => m.IsCaptain)
                .ThenBy(m => m.JoinedAt)
                .ToListAsync();
    }
}
