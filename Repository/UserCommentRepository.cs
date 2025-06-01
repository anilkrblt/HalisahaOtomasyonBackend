using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class UserCommentRepository
        : RepositoryBase<UserComment>, IUserCommentRepository
    {
        public UserCommentRepository(RepositoryContext context)
            : base(context) { }

        /* CREATE / DELETE */
        public void CreateUserComment(UserComment comment) => Create(comment);
        public void DeleteUserComment(UserComment comment) => Delete(comment);

        /* READ – tüm yorumlar */
        private static IQueryable<UserComment> NotDeleted(IQueryable<UserComment> q) =>
            q.Where(c => !c.IsDeleted);

        public async Task<IEnumerable<UserComment>> GetAllUserCommentsAsync(bool trackChanges) =>
            await NotDeleted(FindAll(trackChanges))
                .Include(c => c.FromUser)
                .Include(c => c.ToUser)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        public async Task<UserComment?> GetUserCommentAsync(int id, bool trackChanges) =>
            await NotDeleted(
                    FindByCondition(c => c.Id == id, trackChanges)
                    .Include(c => c.FromUser)
                    .Include(c => c.ToUser))
                .SingleOrDefaultAsync();


        /* READ – filtreler */
        public async Task<IEnumerable<UserComment>> GetCommentsAboutUserAsync(int toUserId, bool trackChanges) =>
            await FindByCondition(c => c.ToUserId == toUserId, trackChanges)
                  .OrderByDescending(c => c.CreatedAt)
                  .ToListAsync();

        public async Task<IEnumerable<UserComment>> GetCommentsFromUserAsync(int fromUserId, bool trackChanges) =>
            await FindByCondition(c => c.FromUserId == fromUserId, trackChanges)
                  .OrderByDescending(c => c.CreatedAt)
                  .ToListAsync();
    }
}
