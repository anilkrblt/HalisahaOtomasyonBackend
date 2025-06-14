using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IFriendshipRepository
    {
        /* Command */

        Task<IEnumerable<Friendship>> GetSentRequestsAsync(int fromUserId, bool trackChanges);

        void CreateFriendship(Friendship friendship);
        void DeleteFriendship(Friendship friendship);

        Task<Friendship?> GetFriendshipAsync(int userIdA, int userIdB, bool trackChanges);


        Task<bool> DoesUserExistAsync(int userId);


        Task<Customer?> GetCustomerByUserNameAsync(string userName);
        Task<IEnumerable<Customer>> SearchCustomersAsync(string q, int take = 10);
        Task<IEnumerable<Friendship>> GetFriendsOfUserAsync(int userId, bool trackChanges);          // Status = Accepted
        Task<IEnumerable<Friendship>> GetPendingRequestsForUserAsync(int userId, bool trackChanges); // userId alıcı
    }
}
