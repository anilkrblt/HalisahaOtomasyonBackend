using Entities.Models;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamMiniDto>> GetTeamsUserIsAdminOfAsync(int userId);
        Task<TeamDto> CreateTeamAsync(TeamForCreationDto dto, int creatorUserId);
        Task<TeamDto> GetTeamAsync(int teamId, int reviewerId, bool trackChanges);
        Task<IEnumerable<TeamDto>> GetTeamsAsync(string? city, string? teamName, bool trackChanges);
        Task<TeamDto> UpdateTeamAsync(int teamId, TeamForUpdateDto dto);
        Task DeleteTeamAsync(int teamId);
        Task AddMembersAsync(int teamId, List<TeamMemberDtoForAdd> dtos);
        Task RemoveMemberAsync(int teamId, int userId);
        Task<IEnumerable<TeamMemberDto>> GetMembersAsync(int teamId, bool trackChanges);
        Task<TeamJoinRequestDto> CreateJoinRequestAsync(int teamId, int userId);
        Task RespondJoinRequestAsync(int teamId, int requestId, TeamJoinRequestDtoForUpdate dto, int responderId);
        Task<IEnumerable<TeamJoinRequestDto>> GetTeamJoinRequestsAsync(int teamId, bool trackChanges);
        Task<IEnumerable<TeamJoinRequestDto>> GetUserJoinRequestsAsync(int userId, bool trackChanges);
        Task<TeamMemberDto> SetAdminAndCaptain(int teamId, int userId, TeamMemberDtoForUpdateAdminAndCaptain teamMemberDto);
    }
}
