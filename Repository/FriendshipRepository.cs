using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class FriendshipRepository
        : RepositoryBase<Friendship>, IFriendshipRepository
    {
        public FriendshipRepository(RepositoryContext ctx) : base(ctx) { }


        /* ---- sadece Customer alt tiplerini getiren yardımcı IQueryable ---- */
        private IQueryable<Customer> CustomerSet => RepositoryContext.Users.OfType<Customer>();


        /* ---------- Command ---------- */
        public void CreateFriendship(Friendship friendship) => Create(friendship);
        public void DeleteFriendship(Friendship friendship) => Delete(friendship);

        /* ---------- Tek satır ---------- */
        public async Task<Friendship?> GetFriendshipAsync(int userIdA, int userIdB, bool trackChanges)
        {
            int a = userIdA < userIdB ? userIdA : userIdB;
            int b = userIdA < userIdB ? userIdB : userIdA;

            return await FindByCondition(f => f.UserId1 == a && f.UserId2 == b, trackChanges)
                         .Include(f => f.User1)
                         .Include(f => f.User2)
                         .SingleOrDefaultAsync();
        }


        public async Task<IEnumerable<Friendship>> GetSentRequestsAsync(int fromUserId, bool trackChanges)
        {
            return await FindByCondition(f =>
                f.UserId1 == fromUserId && f.Status == FriendshipStatus.Pending,
                trackChanges)
                .Include(f => f.User2)  // Alıcı kullanıcı bilgilerini dahil et
                .ToListAsync();
        }



        public async Task<bool> DoesUserExistAsync(int userId) =>
            await RepositoryContext.Users.AnyAsync(u => u.Id == userId);


        /* ---------- Accepted listesi ---------- */
        public async Task<IEnumerable<Friendship>> GetFriendsOfUserAsync(int userId, bool trackChanges) =>
            await FindByCondition(f =>
                        f.Status == FriendshipStatus.Accepted &&
                       (f.UserId1 == userId || f.UserId2 == userId),
                        trackChanges)
                  .Include(f => f.User1)
                  .Include(f => f.User2)
                  .OrderByDescending(f => f.CreatedAt)
                  .ToListAsync();

        /* ---------- Bekleyen istekler ---------- */
        public async Task<IEnumerable<Friendship>> GetPendingRequestsForUserAsync(int userId, bool trackChanges) =>
            await FindByCondition(f =>
                        f.Status == FriendshipStatus.Pending &&
                        f.UserId2 == userId,               // alıcı
                        trackChanges)
                  .Include(f => f.User1)                  // göndereni göster
                  .OrderByDescending(f => f.CreatedAt)
                  .ToListAsync();




        public async Task<Customer?> GetCustomerByUserNameAsync(string userName) =>
            await CustomerSet.SingleOrDefaultAsync(c => c.UserName == userName);

        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string q, int take = 10) =>
                await CustomerSet
                     .Where(c => EF.Functions.Like(c.UserName, $"%{q}%"))
                     .OrderBy(c => c.UserName)
                     .Take(take)
                     .ToListAsync();

    }
}
