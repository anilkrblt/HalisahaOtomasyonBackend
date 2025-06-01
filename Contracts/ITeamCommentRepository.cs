// Contracts/ITeamCommentRepository.cs
using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITeamCommentRepository
    {
        void CreateTeamComment(TeamComment comment);
        void DeleteTeamComment(TeamComment comment);

        Task<IEnumerable<TeamComment>> GetAllTeamCommentsAsync(bool trackChanges);
        Task<TeamComment?> GetTeamCommentAsync(int commentId, bool trackChanges);
        Task<IEnumerable<TeamComment>> GetTeamCommentsByTeamIdAsync(int teamId, bool trackChanges);
    }
}
