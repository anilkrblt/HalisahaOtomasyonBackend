// Service.Contracts/IMatchService.cs
using Entities.Models;
using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface IMatchService
{
    /*──── Match CRUD ────*/
    Task<MatchDto> CreateMatchAsync(MatchForCreationDto dto);
    Task<MatchDto> GetMatchAsync(int matchId, bool trackChanges);
    Task<IEnumerable<MatchDto>> GetAllMatchesAsync(bool trackChanges);
    Task<IEnumerable<MatchDto>> GetMatchesByFieldIdAsync(int fieldId, bool trackChanges);
    Task<IEnumerable<MatchDto>> GetMatchesByTeamIdAsync(int teamId, bool trackChanges);
    Task UpdateMatchAsync(int matchId, MatchForUpdateDto dto);
    Task DeleteMatchAsync(int matchId);

    /*──── Match Request CRUD ────*/
    Task<MatchRequestDto> CreateMatchRequestAsync(MatchRequestForCreationDto dto);
    Task RespondMatchRequestAsync(int requestId, RequestStatus status);
    Task<IEnumerable<MatchRequestDto>> GetRequestsSentByUserAsync(int userId, bool trackChanges);
    Task<IEnumerable<MatchRequestDto>> GetRequestsReceivedByUserAsync(int userId, bool trackChanges);
    Task DeleteMatchRequestAsync(int requestId);
}
