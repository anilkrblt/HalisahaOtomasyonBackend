using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class MatchService : IMatchService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _map;

    public MatchService(IRepositoryManager repo, IMapper mapper)
    {
        _repo = repo;
        _map = mapper;
    }

    /*────────  GET  ─────────────────────────────────────────*/

    public async Task<MatchDto> GetMatchAsync(int matchId, bool trackChanges) =>
        _map.Map<MatchDto>(
            await _repo.Match.GetMatchAsync(matchId, trackChanges)
            ?? throw new MatchNotFoundException(matchId));

    public async Task<IEnumerable<MatchDto>> GetMatchesByFieldIdAsync(int fieldId, bool trackChanges) =>
        _map.Map<IEnumerable<MatchDto>>(
            await _repo.Match.GetMatchesByFieldIdAsync(fieldId, trackChanges));

    public async Task<IEnumerable<MatchDto>> GetMatchesByTeamIdAsync(int teamId, bool trackChanges) =>
        _map.Map<IEnumerable<MatchDto>>(
            await _repo.Match.GetMatchesByTeamIdAsync(teamId, trackChanges));

    /*────────  SKOR GÜNCELLE  ──────────────────────────────*/

    public async Task UpdateScoreAsync(int matchId, ScoreUpdateDto dto)
    {
        var match = await _repo.Match.GetMatchAsync(matchId, true)
                    ?? throw new MatchNotFoundException(matchId);

        match.HomeScore = dto.Home;
        match.AwayScore = dto.Away;

        await _repo.SaveAsync();
    }
}
