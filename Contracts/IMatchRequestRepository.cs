using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IMatchRequestRepository
    {
        /* Command */
        void CreateMatchRequest(MatchRequest matchRequest);
        void DeleteMatchRequest(MatchRequest matchRequest);

        /* Query */
        Task<IEnumerable<MatchRequest>> GetAllMatchRequestsAsync(bool trackChanges);
        Task<MatchRequest?>             GetMatchRequestAsync(int matchRequestId, bool trackChanges);

        /* Ek: kaptan bazlı sorgular */
        Task<IEnumerable<MatchRequest>> GetRequestsSentByUserAsync(int userId, bool trackChanges);
        Task<IEnumerable<MatchRequest>> GetRequestsReceivedByUserAsync(int userId, bool trackChanges);
    }
}
