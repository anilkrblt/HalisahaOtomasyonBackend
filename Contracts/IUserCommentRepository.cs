// Contracts/IUserCommentRepository.cs  (güncellenmiş)
using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IUserCommentRepository
    {
        void CreateUserComment(UserComment comment);
        void DeleteUserComment(UserComment comment);

        Task<IEnumerable<UserComment>> GetAllUserCommentsAsync(bool trackChanges);
        Task<UserComment?> GetUserCommentAsync(int commentId, bool trackChanges);

        /* Hedefe göre filtreler */
        Task<IEnumerable<UserComment>> GetCommentsAboutUserAsync(int toUserId, bool trackChanges);   // alan kullanıcıya gelen
        Task<IEnumerable<UserComment>> GetCommentsFromUserAsync(int fromUserId, bool trackChanges);  // kullanıcının yazdığı
    }
}
