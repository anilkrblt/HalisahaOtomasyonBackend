using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class CommentRepository
        : RepositoryBase<Comment>, ICommentRepository
    {
        public CommentRepository(RepositoryContext context) : base(context) { }

        /* -------------------- Command -------------------- */
        public void CreateComment(Comment comment) => Create(comment);
        public void DeleteComment(Comment comment) => Delete(comment);

        /* -------------------- Query helpers -------------- */
        private static IQueryable<Comment> NotDeleted(IQueryable<Comment> q) =>
            q.Where(c => !c.IsDeleted);

        /* -------------------- Query: All ----------------- */
        public async Task<IEnumerable<Comment>> GetAllCommentsAsync(bool trackChanges) =>
            await NotDeleted(FindAll(trackChanges))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        /* -------------------- Query: By Id --------------- */
        public async Task<Comment?> GetOneCommentAsync(int commentId, bool trackChanges) =>
            await NotDeleted(FindByCondition(c => c.Id == commentId, trackChanges))
                .SingleOrDefaultAsync();

        /* -------------------- Query: Field --------------- */
        public async Task<IEnumerable<Comment>> GetCommentsByFieldIdAsync(int fieldId, bool trackChanges) =>
            await NotDeleted(FindByCondition(
                    c => c.TargetType == CommentTargetType.Field &&
                         c.TargetId == fieldId, trackChanges))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        /* -------------------- Query: Team ---------------- */
        public async Task<IEnumerable<Comment>> GetCommentsByTeamIdAsync(int teamId, bool trackChanges) =>
            await NotDeleted(FindByCondition(
                    c => c.TargetType == CommentTargetType.Team &&
                         c.TargetId == teamId, trackChanges))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        /* -------------------- Query: User ---------------- */
        public async Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId, bool trackChanges) =>
            await NotDeleted(FindByCondition(
                    c => c.TargetType == CommentTargetType.User &&
                         c.TargetId == userId, trackChanges))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

        /* ------------ Query: Facility (join) ------------- */
        public async Task<IEnumerable<Comment>> GetCommentsByFacilityIdAsync(int facilityId, bool trackChanges)
        {
            // Yalnızca saha yorumları (TargetType = Field) tesisle eşleştirilir
            var query = from c in RepositoryContext.Comments
                        join f in RepositoryContext.Fields on c.TargetId equals f.Id
                        where c.TargetType == CommentTargetType.Field &&
                              f.FacilityId == facilityId &&
                              !c.IsDeleted
                        select c;

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
