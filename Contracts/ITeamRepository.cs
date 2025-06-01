using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITeamRepository
    {
        /* Command */
        void CreateTeam(Team team);
        void DeleteTeam(Team team);

        /* Query */
        Task<IEnumerable<Team>> GetAllTeamsAsync(bool trackChanges);
        Task<Team?> GetTeamAsync(int teamId, bool trackChanges);

        /* Ek sorgular (isteğe bağlı) */
        Task<IEnumerable<Team>> GetTeamsByCityAsync(string city, bool trackChanges);
        Task<IEnumerable<Team>> SearchTeamsByNameAsync(string keyword, bool trackChanges);
    }
}
