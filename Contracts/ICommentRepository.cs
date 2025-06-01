using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICommentRepository
    {
        /* Command */
        void CreateComment(Comment comment);
        void DeleteComment(Comment comment);

        /* Query */
        Task<IEnumerable<Comment>> GetAllCommentsAsync(bool trackChanges);
        Task<Comment?> GetOneCommentAsync(int commentId, bool trackChanges);

        Task<IEnumerable<Comment>> GetCommentsByFieldIdAsync(int fieldId, bool trackChanges);
        Task<IEnumerable<Comment>> GetCommentsByTeamIdAsync(int teamId, bool trackChanges);
        Task<IEnumerable<Comment>> GetCommentsByUserIdAsync(int userId, bool trackChanges);
        Task<IEnumerable<Comment>> GetCommentsByFacilityIdAsync(int facilityId, bool trackChanges);
    }
}
