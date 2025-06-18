using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITeamMemberRepository
    {
        void AddMember(TeamMember member);
        void RemoveMember(TeamMember member);
        Task<TeamMember?> GetMemberAsync(int teamId, int userId, bool trackChanges);
        Task<TeamMember> GetTeamMemberAsync(int teamId, int userId, bool trackChanges);
        Task<IEnumerable<TeamMember>> GetMembersByTeamIdWithUserAsync(int teamId, bool trackChanges);
        Task<IEnumerable<TeamMember>> GetMembersByTeamIdAsync(int teamId, bool trackChanges);
        Task<IEnumerable<TeamMember>> GetTeamsByUserIdAsync(int userId, bool trackChanges);
        Task<bool> ExistsAsync(int teamId, int userId);
    }
}
