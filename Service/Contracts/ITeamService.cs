// Service.Contracts/ITeamService.cs
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface ITeamService
    {
        // Service.Contracts/ITeamService.cs

        Task SetTeamLogoAsync(int teamId, IFormFile logoFile);

        /*────────────────────  TEAM  ────────────────────*/
        // Service.Contracts/ITeamService.cs
        Task<TeamDto> CreateTeamAsync(TeamForCreationDto dto, int creatorUserId);
        Task<TeamDto> GetTeamAsync(int teamId, bool trackChanges);
        Task<IEnumerable<TeamDto>> GetAllTeamsAsync(bool trackChanges);
        Task<IEnumerable<TeamDto>> GetTeamsByCityAsync(string city, bool trackChanges);
        Task<IEnumerable<TeamDto>> SearchTeamsByNameAsync(string keyword, bool trackChanges);
        Task<TeamDto> UpdateTeamAsync(int teamId, TeamForUpdateDto dto);
        Task DeleteTeamAsync(int teamId);

        /*─────────────────  TEAM MEMBERS  ───────────────*/
        Task AddMemberAsync(int teamId, int userId, PlayerPosition pos, bool isCaptain);
        Task RemoveMemberAsync(int teamId, int userId);
        Task<IEnumerable<TeamMemberDto>> GetMembersAsync(int teamId, bool trackChanges);
        Task<IEnumerable<TeamDto>> GetTeamsOfUserAsync(int userId, bool trackChanges);

        /*───────────────  JOIN REQUESTS  ────────────────*/
        Task<TeamJoinRequestDto> CreateJoinRequestAsync(int teamId, int userId);
        Task RespondJoinRequestAsync(int requestId, RequestStatus status, int responderId);
        Task<IEnumerable<TeamJoinRequestDto>> GetTeamJoinRequestsAsync(int teamId, bool trackChanges);
        Task<IEnumerable<TeamJoinRequestDto>> GetUserJoinRequestsAsync(int userId, bool trackChanges);
        Task<TeamMemberDto> SetAdminAndCaptain(int teamId, int userId, TeamMemberDtoForUpdateAdminAndCaptain teamMemberDto);
    }
}
