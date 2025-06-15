using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;

namespace Repository
{
    public class TeamRepository : RepositoryBase<Team>, ITeamRepository
    {
        public TeamRepository(RepositoryContext ctx) : base(ctx) { }
        public void CreateTeam(Team team) => Create(team);
        public void DeleteTeam(Team team) => Delete(team);

        public async Task<Team> GetTeamAsync(int teamId, bool trackChanges) =>
            await FindAll(trackChanges)
                .IncludeMembers()
                .SingleOrDefaultAsync(t => t.Id == teamId);

        public async Task<IEnumerable<Team>> GetTeamsAsync(string? city, string? teamName, bool trackChanges) =>
            await FindAll(trackChanges)
            .Search(teamName, city)
            .IncludeMembers()
            .OrderBy(t => t.Name)
            .ToListAsync();
    }
}
