using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITeamRepository
    {
        void CreateTeam(Team team);
        void DeleteTeam(Team team);
        Task<Team> GetTeamAsync(int teamId, bool trackChanges);
        Task<IEnumerable<Team>> GetTeamsAsync(string city, string teamName, bool trackChanges);
    }
}
