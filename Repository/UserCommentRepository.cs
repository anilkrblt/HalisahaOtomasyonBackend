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

        public void CreateUserComment(UserComment comment) => Create(comment);
        public void DeleteUserComment(UserComment comment) => Delete(comment);
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
                    FindByCondition(c => c.Id == id, trackChanges))
                    .Include(c => c.FromUser)
                    .Include(c => c.ToUser)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<UserComment>> GetCommentsAboutUserAsync(int toUserId, bool trackChanges) =>
            await NotDeleted(
                     FindByCondition(c => c.ToUserId == toUserId, trackChanges))
                    .Include(c => c.FromUser)
                    .Include(c => c.ToUser)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

        public async Task<IEnumerable<UserComment>> GetCommentsFromUserAsync(int fromUserId, bool trackChanges) =>
            await NotDeleted(FindByCondition(c => c.FromUserId == fromUserId, trackChanges))
                  .Include(c => c.FromUser)
                  .Include(c => c.ToUser)
                  .OrderByDescending(c => c.CreatedAt)
                  .ToListAsync();
    }
}
