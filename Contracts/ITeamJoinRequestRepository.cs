using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITeamJoinRequestRepository
    {
        /* Command */
        void CreateJoinRequest (TeamJoinRequest request);
        void DeleteJoinRequest (TeamJoinRequest request);

        /* Query – tek kayıt */
        Task<TeamJoinRequest?> GetJoinRequestAsync(int requestId, bool trackChanges);

        /* Query – listeler */
        Task<IEnumerable<TeamJoinRequest>> GetRequestsByTeamIdAsync(int teamId, bool trackChanges);
        Task<IEnumerable<TeamJoinRequest>> GetRequestsByUserIdAsync(int userId, bool trackChanges);
        Task<IEnumerable<TeamJoinRequest>> GetPendingRequestsByTeamIdAsync(int teamId, bool trackChanges);
    }
}
