using Entities.Models;

namespace Contracts
{
    public interface IFieldCommentRepository
    {
        Task<IEnumerable<FieldComment>> GetCommentsForFieldsAsync(IEnumerable<int> fieldIds);
        void CreateFieldComment(FieldComment comment);
        Task<IEnumerable<FieldComment>> GetAllFieldCommentsAsync(bool trackChanges);
        Task<FieldComment?> GetFieldCommentAsync(int commentId, bool trackChanges);
        Task<IEnumerable<FieldComment>> GetFieldCommentsByFieldIdAsync(int fieldId, bool trackChanges);
    }
}
