using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class FieldCommentRepository
        : RepositoryBase<FieldComment>, IFieldCommentRepository
    {
        public FieldCommentRepository(RepositoryContext context)
            : base(context) { }

        public void CreateFieldComment(FieldComment comment) => Create(comment);

        public async Task<IEnumerable<FieldComment>> GetAllFieldCommentsAsync(bool trackChanges) =>
            await FindByCondition(c => !c.IsDeleted, trackChanges)
                  .Include(c => c.FromUser)
                  .OrderBy(c => c.CreatedAt)
                  .ToListAsync();

        public async Task<FieldComment?> GetFieldCommentAsync(int commentId, bool trackChanges) =>
            await FindByCondition(c => c.Id == commentId && !c.IsDeleted, trackChanges)
                  .Include(c => c.FromUser)
                  .SingleOrDefaultAsync();

        public async Task<IEnumerable<FieldComment>> GetFieldCommentsByFieldIdAsync(int fieldId, bool trackChanges) =>
            await FindByCondition(c => c.FieldId == fieldId && !c.IsDeleted, trackChanges)
                  .Include(c => c.FromUser)
                  .OrderByDescending(c => c.CreatedAt)
                  .ToListAsync();
        public async Task<IEnumerable<FieldComment>> GetCommentsForFieldsAsync(IEnumerable<int> fieldIds)
        {
            return await FindByCondition(
                    c => fieldIds.Contains(c.FieldId) && !c.IsDeleted,
                    trackChanges: false)
                .Include(c => c.FromUser)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

    }
}
