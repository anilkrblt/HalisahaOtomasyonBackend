using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class MatchService : IMatchService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _mapper;

    public MatchService(IRepositoryManager repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    /*──── MATCH CRUD ───────────────────────────────────────────*/

    public async Task<MatchDto> CreateMatchAsync(MatchForCreationDto dto)
    {
        var entity = _mapper.Map<Match>(dto);
        _repo.Match.CreateMatch(entity);
        await _repo.SaveAsync();
        return _mapper.Map<MatchDto>(entity);
    }

    public async Task<MatchDto> GetMatchAsync(int id, bool track) =>
        _mapper.Map<MatchDto>(
            await _repo.Match.GetMatchAsync(id, track)
            ?? throw new MatchNotFoundException(id));

    public async Task<IEnumerable<MatchDto>> GetAllMatchesAsync(bool track) =>
        _mapper.Map<IEnumerable<MatchDto>>(
            await _repo.Match.GetAllMatchesAsync(track));

    public async Task<IEnumerable<MatchDto>> GetMatchesByFieldIdAsync(int fieldId, bool track) =>
        _mapper.Map<IEnumerable<MatchDto>>(
            await _repo.Match.GetMatchesByFieldIdAsync(fieldId, track));

    public async Task<IEnumerable<MatchDto>> GetMatchesByTeamIdAsync(int teamId, bool track) =>
        _mapper.Map<IEnumerable<MatchDto>>(
            await _repo.Match.GetMatchesByTeamIdAsync(teamId, track));

    public async Task UpdateMatchAsync(int id, MatchForUpdateDto dto)
    {
        var entity = await _repo.Match.GetMatchAsync(id, true)
                     ?? throw new MatchNotFoundException(id);

        // Null-kontrollü güncelleme
        if (dto.HomeScore.HasValue) entity.HomeScore = dto.HomeScore.Value;
        if (dto.AwayScore.HasValue) entity.AwayScore = dto.AwayScore.Value;
        if (dto.DateTime.HasValue) entity.DateTime = dto.DateTime.Value;
        if (dto.FieldId.HasValue) entity.FieldId = dto.FieldId.Value;

        await _repo.SaveAsync();
    }

    public async Task DeleteMatchAsync(int id)
    {
        var entity = await _repo.Match.GetMatchAsync(id, true)
                     ?? throw new MatchNotFoundException(id);

        _repo.Match.DeleteMatch(entity);
        await _repo.SaveAsync();
    }

    /*──── MATCH REQUEST CRUD ────────────────────────────────*/

    public async Task<MatchRequestDto> CreateMatchRequestAsync(MatchRequestForCreationDto dto)
    {
        var entity = _mapper.Map<MatchRequest>(dto);
        _repo.MatchRequest.CreateMatchRequest(entity);
        await _repo.SaveAsync();
        return _mapper.Map<MatchRequestDto>(entity);
    }

    public async Task RespondMatchRequestAsync(int id, RequestStatus status)
    {
        var req = await _repo.MatchRequest.GetMatchRequestAsync(id, true)
                  ?? throw new MatchRequestNotFoundException(id);

        req.Status = status;
        req.RespondedAt = DateTime.UtcNow;
        await _repo.SaveAsync();
    }

    public async Task<IEnumerable<MatchRequestDto>>
        GetRequestsSentByUserAsync(int userId, bool track) =>
        _mapper.Map<IEnumerable<MatchRequestDto>>(
            await _repo.MatchRequest.GetRequestsSentByUserAsync(userId, track));

    public async Task<IEnumerable<MatchRequestDto>>
        GetRequestsReceivedByUserAsync(int userId, bool track) =>
        _mapper.Map<IEnumerable<MatchRequestDto>>(
            await _repo.MatchRequest.GetRequestsReceivedByUserAsync(userId, track));

    public async Task DeleteMatchRequestAsync(int id)
    {
        var req = await _repo.MatchRequest.GetMatchRequestAsync(id, true)
                  ?? throw new MatchRequestNotFoundException(id);

        _repo.MatchRequest.DeleteMatchRequest(req);
        await _repo.SaveAsync();
    }
}
