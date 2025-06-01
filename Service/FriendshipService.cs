// Service/FriendshipService.cs
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class FriendshipService : IFriendshipService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _mapper;

    public FriendshipService(IRepositoryManager repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }


    public async Task<IEnumerable<CustomerLiteDto>> SearchCustomersAsync(string q, int take)
    {
        var users = await _repo.Friendship.SearchCustomersAsync(q, take);
        return users.Select(u => new CustomerLiteDto(
            u.Id,
            u.UserName,
            string.IsNullOrWhiteSpace($"{u.FirstName} {u.LastName}".Trim()) ? null : $"{u.FirstName} {u.LastName}".Trim()));
    }

    /*──── Send Request ────*/

    public async Task<FriendshipDto> SendFriendRequestAsync(int fromUserId, int toUserId)
    {
        if (fromUserId == toUserId)
            throw new InvalidOperationException("Kendinize istek gönderemezsiniz.");

        /* yalnızca var olan kullanıcılar */
        if (!await _repo.Friendship.DoesUserExistAsync(toUserId))
            throw new InvalidOperationException("Hedef kullanıcı bulunamadı.");

        /* mevcut satır? */
        var row = await _repo.Friendship.GetFriendshipAsync(fromUserId, toUserId, true);
        if (row is not null)
        {
            if (row.Status == FriendshipStatus.Pending)
                throw new InvalidOperationException("Zaten bekleyen bir istek var.");
            if (row.Status == FriendshipStatus.Accepted)
                throw new InvalidOperationException("Zaten arkadaşsınız.");

            row.Status = FriendshipStatus.Pending;
            row.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            row = new Friendship
            {
                UserId1 = Math.Min(fromUserId, toUserId),
                UserId2 = Math.Max(fromUserId, toUserId),
                Status = FriendshipStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _repo.Friendship.CreateFriendship(row);
        }

        await _repo.SaveAsync();
        return _mapper.Map<FriendshipDto>(row);
    }

    /*──── Respond ────*/
    public async Task RespondFriendRequestAsync(int userIdA, int userIdB, FriendshipStatus status)
    {
        var row = await _repo.Friendship.GetFriendshipAsync(userIdA, userIdB, true)
                  ?? throw new FriendshipNotFoundException(userIdA, userIdB);

        /* sadece alıcı kabul/ret edebilir */
        if (row.UserId2 != userIdA && row.UserId2 != userIdB)
            throw new InvalidOperationException("Bu isteği yanıtlamaya yetkiniz yok.");

        if (row.Status != FriendshipStatus.Pending)
            throw new InvalidOperationException("Bu isteğin durumu zaten değişmiş.");

        row.Status = status;
        row.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveAsync();
    }

    /*──── Cancel Pending (gönderen iptal) ────*/
    public async Task CancelFriendRequestAsync(int fromUserId, int toUserId)
    {
        var row = await _repo.Friendship.GetFriendshipAsync(fromUserId, toUserId, true)
                  ?? throw new FriendshipNotFoundException(fromUserId, toUserId);

        if (row.Status != FriendshipStatus.Pending || row.UserId1 != Math.Min(fromUserId, toUserId))
            throw new InvalidOperationException("İptal edilecek bekleyen istek bulunamadı.");

        _repo.Friendship.DeleteFriendship(row);
        await _repo.SaveAsync();
    }




    /*──── Delete ────*/
    public async Task DeleteFriendAsync(int userIdA, int userIdB)
    {
        var row = await _repo.Friendship.GetFriendshipAsync(userIdA, userIdB, true)
                  ?? throw new FriendshipNotFoundException(userIdA, userIdB);

        _repo.Friendship.DeleteFriendship(row);
        await _repo.SaveAsync();
    }

    /*──── Queries ────*/
    public async Task<IEnumerable<FriendshipDto>> GetFriendsAsync(int userId, bool track) =>
        _mapper.Map<IEnumerable<FriendshipDto>>(
            await _repo.Friendship.GetFriendsOfUserAsync(userId, track));

    public async Task<IEnumerable<FriendshipDto>> GetPendingRequestsAsync(int userId, bool track) =>
        _mapper.Map<IEnumerable<FriendshipDto>>(
            await _repo.Friendship.GetPendingRequestsForUserAsync(userId, track));
}

/* Basit istisna */
public sealed class FriendshipNotFoundException : NotFoundException
{
    public FriendshipNotFoundException(int a, int b)
        : base($"Friendship not found between users {a} & {b}.") { }
}
