using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IMatchService
    {
        /*────────  GET  ────────*/
        Task<MatchDto> GetMatchAsync(int matchId, bool trackChanges);
        Task<IEnumerable<MatchDto>> GetMatchesByFieldIdAsync(int fieldId, bool trackChanges);
        Task<IEnumerable<MatchDto>> GetMatchesByTeamIdAsync(int teamId, bool trackChanges);

        /*────────  SCORE UPDATE  ────────*/
        Task UpdateScoreAsync(int matchId, ScoreUpdateDto dto);
    }
}
