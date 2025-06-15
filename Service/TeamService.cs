using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Service.Contracts;
using Shared.DataTransferObjects;
using Stripe;

namespace Service;

public class TeamService : ITeamService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _log;
    private readonly IPhotoService _photoService;

    public TeamService(IRepositoryManager repo,
                       IMapper mapper,
                       ILoggerManager log,
                       IPhotoService photoService)
    {
        _photoService = photoService;
        _repo = repo;
        _mapper = mapper;
        _log = log;
    }

    public async Task<TeamDto> CreateTeamAsync(TeamForCreationDto dto, int creatorUserId)
    {
        var entity = _mapper.Map<Team>(dto);
        _repo.Team.CreateTeam(entity);
        await _repo.SaveAsync(); 

        var captain = new TeamMember
        {
            TeamId = entity.Id,
            UserId = creatorUserId,
            IsCaptain = true,
            IsAdmin = true,
            Position = PlayerPosition.Utility, 
            JoinedAt = DateTime.UtcNow
        };
        _repo.TeamMember.AddMember(captain);
        await _repo.SaveAsync();

        var fullTeam = await _repo.Team.GetTeamAsync(entity.Id, trackChanges: false);
        return _mapper.Map<TeamDto>(fullTeam);
    }

    public async Task<TeamDto> GetTeamAsync(int teamId, bool track)
    {
        var members = await _repo.Team.GetTeamAsync(teamId, track)
                         ?? throw new TeamNotFoundException(teamId);

        var teamDto = _mapper.Map<TeamDto>(members);

        foreach (var memberDto in teamDto.Members)
        {
            var photosDto = await _photoService.GetPhotosAsync("user", memberDto.UserId, false);
            memberDto.UserPhotoUrl = photosDto?.FirstOrDefault()?.Url ?? "";
        }

        return teamDto;
    }

    public async Task<IEnumerable<TeamDto>> GetTeamsAsync(string? city, string? teamName, bool trackChanges)
    {
        var teams = await _repo.Team.GetTeamsAsync(city, teamName, trackChanges);
        var teamsDto = _mapper.Map<List<TeamDto>>(teams);

        foreach (var teamDto in teamsDto)
        {
            foreach (var memberDto in teamDto.Members)
            {
                var photosDto = await _photoService.GetPhotosAsync("user", memberDto.UserId, false);
                memberDto.UserPhotoUrl = photosDto?.FirstOrDefault()?.Url ?? "";
            }
        }

        return teamsDto;
    }

    public async Task<TeamDto> UpdateTeamAsync(int id, TeamForUpdateDto dto)
    {
        var entity = await _repo.Team.GetTeamAsync(id, true)
                     ?? throw new TeamNotFoundException(id);

        var url = await _photoService.UploadLogoAsync(dto.Logo, $"team/{id}");

        _mapper.Map(dto, entity);
        entity.LogoUrl = url;

        await _repo.SaveAsync();
        return _mapper.Map<TeamDto>(entity);
    }

    public async Task DeleteTeamAsync(int id)
    {
        var entity = await _repo.Team.GetTeamAsync(id, true)
                     ?? throw new TeamNotFoundException(id);

        _repo.Team.DeleteTeam(entity);
        await _repo.SaveAsync();
    }

    public async Task AddMembersAsync(int teamId, List<TeamMemberDtoForAdd> dtos)
    {
        var team = await _repo.Team.GetTeamAsync(teamId, true)
                   ?? throw new TeamNotFoundException(teamId);

        var members = await _repo.
            TeamMember
            .GetMembersByTeamIdAsync(teamId, false);

        var memberIds = members.Select(m => m.UserId).ToHashSet();
        var duplicateUserIds = dtos.Select(d => d.UserId).Where(id => memberIds.Contains(id)).ToList();
        if (duplicateUserIds.Any())
            throw new InvalidOperationException($"Üye zaten takımda: {string.Join(", ", duplicateUserIds)}");

        var newMembers = _mapper.Map<List<TeamMember>>(dtos);

        foreach (var newMember in newMembers)
        {
            newMember.TeamId = teamId;
            _repo.TeamMember.AddMember(newMember);
        }

        await _repo.SaveAsync();
    }

    public async Task RemoveMemberAsync(int teamId, int userId)
    {
        var member = await _repo.TeamMember.GetMemberAsync(teamId, userId, true)
                     ?? throw new TeamMemberNotFoundException(teamId, userId);

        _repo.TeamMember.RemoveMember(member);
        await _repo.SaveAsync();
    }

    public async Task<IEnumerable<TeamMemberDto>> GetMembersAsync(int teamId, bool track)
    {
        var members = await _repo.TeamMember.GetMembersByTeamIdWithUserAsync(teamId, track);
        var membersDto = _mapper.Map<List<TeamMemberDto>>(members);

        foreach (var member in membersDto)
        {
            var photosDto = await _photoService.
                GetPhotosAsync("user", member.UserId, false);

            var photoUrl = photosDto?.FirstOrDefault()?.Url ?? "";
            member.UserPhotoUrl = photoUrl;
        }

        return membersDto;
    }

    public async Task<TeamMemberDto> SetAdminAndCaptain(int teamId, int userId, TeamMemberDtoForUpdateAdminAndCaptain teamMemberDto)
    {
        var member = await _repo.
            TeamMember
            .GetTeamMemberAsync(teamId, userId, true);

        var updatedMember = _mapper.Map(teamMemberDto, member);
        await _repo.SaveAsync();

        var updatedMembersDto = _mapper.Map<TeamMemberDto>(updatedMember);

        var photosDto = await _photoService.
            GetPhotosAsync("user", userId, false);

        var photoUrl = photosDto?.FirstOrDefault()?.Url ?? "";
        updatedMembersDto.UserPhotoUrl = photoUrl;

        return updatedMembersDto;
    }

    public async Task<TeamJoinRequestDto> CreateJoinRequestAsync(int teamId, int userId)
    {
        var team = await _repo.Team.GetTeamAsync(teamId, false)
                   ?? throw new TeamNotFoundException(teamId);

        var pending = (await _repo.TeamJoinRequest
                           .GetPendingRequestsByTeamIdAsync(teamId, true))
                         .FirstOrDefault(r => r.UserId == userId);

        if (pending is not null)
            throw new InvalidOperationException("Bekleyen isteğiniz zaten var.");

        var req = new TeamJoinRequest { TeamId = teamId, UserId = userId };
        _repo.TeamJoinRequest.CreateJoinRequest(req);
        await _repo.SaveAsync();
        return _mapper.Map<TeamJoinRequestDto>(req);
    }

    public async Task RespondJoinRequestAsync(
    int requestId,
    RequestStatus status,
    int responderId)
    {
        // 1) İsteği bul
        var req = await _repo.TeamJoinRequest
                            .GetJoinRequestAsync(requestId, trackChanges: true)
                  ?? throw new JoinRequestNotFoundException(requestId);

        // 2) Yetki kontrolü: yalnızca takımın kaptanı(ları) kabul/red yapabilir
        var isCaptain = await _repo.TeamMember
            .GetMemberAsync(req.TeamId, responderId, trackChanges: false) is TeamMember m
            && m.IsCaptain;
        if (!isCaptain)
            throw new UnauthorizedAccessException("Bu işlemi yalnızca takım kaptanları yapabilir.");

        // 3) Durumu ve respondedAt’i güncelle
        req.Status = status;
        req.RespondedAt = DateTime.UtcNow;

        // 4) Kabul edilmişse üyeliğe aktar
        if (status == RequestStatus.Accepted)
        {
            var member = new TeamMember
            {
                TeamId = req.TeamId,
                UserId = req.UserId,
                IsCaptain = false,
                JoinedAt = DateTime.UtcNow
            };
            _repo.TeamMember.AddMember(member);
        }

        // 5) Kaydet
        await _repo.SaveAsync();
    }

    public async Task<IEnumerable<TeamJoinRequestDto>>
        GetTeamJoinRequestsAsync(int teamId, bool track) =>
        _mapper.Map<IEnumerable<TeamJoinRequestDto>>(
            await _repo.TeamJoinRequest.GetRequestsByTeamIdAsync(teamId, track));

    public async Task<IEnumerable<TeamJoinRequestDto>>
        GetUserJoinRequestsAsync(int userId, bool track) =>
        _mapper.Map<IEnumerable<TeamJoinRequestDto>>(
            await _repo.TeamJoinRequest.GetRequestsByUserIdAsync(userId, track));
}
