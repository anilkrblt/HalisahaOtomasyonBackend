// Contracts/IFieldCommentRepository.cs
using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IFieldCommentRepository
    {
        /* CREATE / DELETE */
        void CreateFieldComment(FieldComment comment);
        void DeleteFieldComment(FieldComment comment);

        /* READ */
        Task<IEnumerable<FieldComment>> GetAllFieldCommentsAsync(bool trackChanges);
        Task<FieldComment?> GetFieldCommentAsync(int commentId, bool trackChanges);
        Task<IEnumerable<FieldComment>> GetFieldCommentsByFieldIdAsync(int fieldId, bool trackChanges);
    }
}
