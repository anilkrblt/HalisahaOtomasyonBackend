using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IMatchRepository
    {
        /* Command */
        void CreateMatch(Match match);
        void DeleteMatch(Match match);

        /* Query */
        Task<IEnumerable<Match>> GetAllMatchesAsync(bool trackChanges);
        Task<Match?>            GetMatchAsync(int matchId, bool trackChanges);

        /* İsteğe bağlı ek sorgular */
        Task<IEnumerable<Match>> GetMatchesByFieldIdAsync(int fieldId, bool trackChanges);
        Task<IEnumerable<Match>> GetMatchesByTeamIdAsync(int teamId, bool trackChanges);  // home veya away
    }
}
