// Service.Contracts/IFriendshipService.cs
using Entities.Models;
using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface IFriendshipService
{
    Task<FriendshipDto> SendFriendRequestAsync(int fromUserId, int toUserId);
    Task RespondFriendRequestAsync(int userIdA, int userIdB, FriendshipStatus status);
    Task DeleteFriendAsync(int userIdA, int userIdB);
    Task CancelFriendRequestAsync(int fromUserId, int toUserId);

    Task<IEnumerable<FriendshipDto>> GetFriendsAsync(int userId, bool trackChanges);
    Task<IEnumerable<FriendshipDto>> GetPendingRequestsAsync(int userId, bool trackChanges);
    Task<IEnumerable<CustomerLiteDto>> SearchCustomersAsync(string q, int take = 10);
}
public record CustomerLiteDto(int Id, string UserName, string? FullName);
