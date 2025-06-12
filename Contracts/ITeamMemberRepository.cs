using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ITeamMemberRepository
    {
        /* Command */
        void AddMember    (TeamMember member);
        void RemoveMember (TeamMember member);

        /* Query : tek satır (bileşik PK) */
        Task<TeamMember?> GetMemberAsync(int teamId, int userId, bool trackChanges);
        Task<TeamMember> GetTeamMemberAsync(int teamId, int userId, bool trackChanges);

        /* Query : listeler */
        Task<IEnumerable<TeamMember>> GetMembersByTeamIdAsync(int teamId, bool trackChanges);
        Task<IEnumerable<TeamMember>> GetTeamsByUserIdAsync (int userId, bool trackChanges);
    }
}
