using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        /* CREATE / DELETE -------------------------------------------------- */
        public void CreateFieldComment(FieldComment comment) => Create(comment);
        public void DeleteFieldComment(FieldComment comment) => Delete(comment);

        /* READ -------------------------------------------------------------- */
        public async Task<IEnumerable<FieldComment>> GetAllFieldCommentsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                  .OrderBy(c => c.CreatedAt)
                  .ToListAsync();

        public async Task<FieldComment?> GetFieldCommentAsync(int commentId, bool trackChanges) =>
            await FindByCondition(c => c.Id == commentId, trackChanges)
                  .SingleOrDefaultAsync();

        public async Task<IEnumerable<FieldComment>> GetFieldCommentsByFieldIdAsync(int fieldId, bool trackChanges) =>
            await FindByCondition(c => c.FieldId == fieldId, trackChanges)
                  .OrderByDescending(c => c.CreatedAt)
                  .ToListAsync();
    }
}
