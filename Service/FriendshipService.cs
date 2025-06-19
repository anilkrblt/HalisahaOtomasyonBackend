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
    private readonly IPhotoService _photoService;

    public FriendshipService(IRepositoryManager repo, IMapper mapper, IPhotoService photoService)
    {
        _repo = repo;
        _mapper = mapper;
        _photoService = photoService;
    }


    public async Task<IEnumerable<UserDto>> GetOutgoingRequestsAsync(int userId, bool trackChanges)
    {
        var requests = await _repo.Friendship.GetSentRequestsAsync(userId, trackChanges);

        var users = requests.Select(f => new UserDto
        {
            Id = f.User2!.Id,
            UserName = f.User2.UserName,
            FullName = $"{f.User2.FirstName} {f.User2.LastName}"
        });

        return users;
    }


    public async Task<string> GetRelationshipStatusAsync(int callerId, int otherUserId)
    {
        if (callerId == otherUserId)
            return "self";

        var friendship = await _repo.Friendship.GetFriendshipExactAsync(callerId, otherUserId, false)
                        ?? await _repo.Friendship.GetFriendshipExactAsync(otherUserId, callerId, false);

        if (friendship is null)
            return "unfriend";

        if (friendship.Status == FriendshipStatus.Accepted)
            return "friend";

        if (friendship.Status == FriendshipStatus.Pending)
        {
            if (friendship.UserId1 == callerId)
                return "pending";       // biz gönderdik
            else
                return "userPending";   // karşı taraf gönderdi
        }

        return "unfriend"; // fallback
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

        // Hedef kullanıcı gerçekten var mı?
        if (!await _repo.Friendship.DoesUserExistAsync(toUserId))
            throw new InvalidOperationException("Hedef kullanıcı bulunamadı.");

        // Hem (from, to) hem (to, from) kontrolleri yapılmalı
        var existing = await _repo.Friendship.GetFriendshipExactAsync(fromUserId, toUserId, true)
                       ?? await _repo.Friendship.GetFriendshipExactAsync(toUserId, fromUserId, true);

        if (existing is not null)
        {
            if (existing.Status == FriendshipStatus.Pending)
                throw new InvalidOperationException("Zaten bekleyen bir istek var.");

            if (existing.Status == FriendshipStatus.Accepted)
                throw new InvalidOperationException("Zaten arkadaşsınız.");

            // Daha önce reddedilmişse yeniden istek atılabilir
            existing.UserId1 = fromUserId;
            existing.UserId2 = toUserId;
            existing.Status = FriendshipStatus.Pending;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            existing = new Friendship
            {
                UserId1 = fromUserId,
                UserId2 = toUserId,
                Status = FriendshipStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _repo.Friendship.CreateFriendship(existing);
        }

        await _repo.SaveAsync();
        return _mapper.Map<FriendshipDto>(existing);
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
        var row = await _repo.Friendship.GetFriendshipExactAsync(fromUserId, toUserId, true)
                  ?? throw new FriendshipNotFoundException(fromUserId, toUserId);

        if (row.Status != FriendshipStatus.Pending)
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
    public async Task<IEnumerable<FriendshipDto>> GetFriendsAsync(int userId, bool track)
    {
        var friendships = await _repo.Friendship.GetFriendsOfUserAsync(userId, track);

        var list = new List<FriendshipDto>();

        foreach (var f in friendships)
        {
            var user = f.UserId1 == userId ? f.User2! : f.User1!;

            var photos = await _photoService.GetPhotosAsync("user", user.Id, true);
            var photoUrl = photos.FirstOrDefault()?.Url;

            var dto = new FriendshipDto
            {
                UserId1 = f.UserId1,
                UserId2 = f.UserId2,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                User1Info = new UserMiniDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    FullName = $"{user.FirstName} {user.LastName}",
                    PhotoUrl = photoUrl
                }
            };

            list.Add(dto);
        }

        return list;
    }

    public async Task<IEnumerable<FriendshipMobilDto>> GetFriendsMobilAsync(int userId, bool track)
    {
        var friendships = await _repo.Friendship.GetFriendsOfUserAsync(userId, track);

        var list = new List<FriendshipMobilDto>();

        foreach (var f in friendships)
        {
            var user = f.UserId1 == userId ? f.User2! : f.User1!;

            var photos = await _photoService.GetPhotosAsync("user", user.Id, true);
            var photoUrl = photos.FirstOrDefault()?.Url;

            var dto = new FriendshipMobilDto
            {
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                FriendUserId = user.Id,
                UserName = user.UserName!,
                FullName = $"{user.FirstName} {user.LastName}",
                PhotoUrl = photoUrl
            };


            list.Add(dto);
        }

        return list;
    }


    public async Task<IEnumerable<FriendshipDto>> GetPendingRequestsAsync(int userId, bool track)
    {
        var requests = await _repo.Friendship.GetPendingRequestsForUserAsync(userId, track);

        var result = new List<FriendshipDto>();

        foreach (var f in requests)
        {
            var user = f.User1!;
            var photos = await _photoService.GetPhotosAsync("user", user.Id, false);
            var photoUrl = photos.FirstOrDefault()?.Url;

            var dto = new FriendshipDto
            {
                UserId1 = f.UserId1,
                UserId2 = f.UserId2,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                User1Info = new UserMiniDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    FullName = $"{user.FirstName} {user.LastName}",
                    PhotoUrl = photoUrl
                }
            };

            result.Add(dto);
        }

        return result;
    }


}





public sealed class FriendshipNotFoundException : NotFoundException
{
    public FriendshipNotFoundException(int a, int b)
        : base($"Friendship not found between users {a} & {b}.") { }
}
