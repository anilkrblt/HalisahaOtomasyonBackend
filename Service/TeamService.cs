using AutoMapper;
using AutoMapper.Execution;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class TeamService : ITeamService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;
    private readonly IUserValidationService _uservalidationService;

    public TeamService(IRepositoryManager repo,
                       IMapper mapper,
                       IPhotoService photoService,
                       IUserValidationService userValidationService)
    {
        _photoService = photoService;
        _repo = repo;
        _mapper = mapper;
        _uservalidationService = userValidationService;
    }


    public async Task<IEnumerable<TeamMiniDto>> GetTeamsUserIsAdminOfAsync(int userId)
    {
        var teams = await _repo.Team.GetTeamsUserIsAdminOfAsync(userId);

        return teams.Select(t => new TeamMiniDto
        {
            Id = t.Id,
            Name = t.Name,
            LogoUrl = t.LogoUrl,
            City = t.City,
            Town = t.Town,
            AvgRating = t.AvgRating
            
        });
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
            Position = "Kaptan",
            JoinedAt = DateTime.UtcNow
        };
        _repo.TeamMember.AddMember(captain);
        await _repo.SaveAsync();

        var fullTeam = await _repo.Team.GetTeamAsync(entity.Id, trackChanges: false);
        return _mapper.Map<TeamDto>(fullTeam);
    }

    public async Task<TeamDto> GetTeamAsync(int teamId, int reviewerId, bool track)
    {
        var members = await CheckTeamExists(teamId);

        var teamDto = _mapper.Map<TeamDto>(members);

        await SetUserStatus(teamDto, teamId, reviewerId);

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

    public async Task<TeamDto> UpdateTeamAsync(int teamId, TeamForUpdateDto dto)
    {
        var entity = await CheckTeamExists(teamId);

        _mapper.Map(dto, entity);

        await _repo.SaveAsync();
        return _mapper.Map<TeamDto>(entity);
    }

    public async Task DeleteTeamAsync(int teamId)
    {
        var entity = await CheckTeamExists(teamId);

        _repo.Team.DeleteTeam(entity);
        await _repo.SaveAsync();
    }

    public async Task<IEnumerable<TeamMemberDto>> GetMembersAsync(int teamId, bool track)
    {
        await CheckTeamExists(teamId);
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

    public async Task AddMembersAsync(int teamId, List<TeamMemberDtoForAdd> dtos)
    {
        foreach (var userId in dtos.Select(tm => tm.UserId))
        {
            await _uservalidationService.CheckUserExists(userId);
        }

        await CheckTeamExists(teamId);

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

    public async Task<TeamMemberDto> SetAdminAndCaptain(int teamId, int userId, TeamMemberDtoForUpdateAdminAndCaptain teamMemberDto)
    {
        await CheckTeamExists(teamId);
        await _uservalidationService.CheckUserExists(userId);

        var member = await CheckTeamMembership(teamId, userId);

        var updatedMember = _mapper.Map(teamMemberDto, member);
        await _repo.SaveAsync();

        var updatedMembersDto = _mapper.Map<TeamMemberDto>(updatedMember);

        var photosDto = await _photoService.
            GetPhotosAsync("user", userId, false);

        var photoUrl = photosDto?.FirstOrDefault()?.Url ?? "";
        updatedMembersDto.UserPhotoUrl = photoUrl;

        return updatedMembersDto;
    }

    public async Task RemoveMemberAsync(int teamId, int userId)
    {
        await CheckTeamExists(teamId);
        await _uservalidationService.CheckUserExists(userId);

        var member = await CheckTeamMembership(teamId, userId);

        _repo.TeamMember.RemoveMember(member);
        await _repo.SaveAsync();
    }

    public async Task<IEnumerable<TeamJoinRequestDto>> GetTeamJoinRequestsAsync(int teamId, bool track)
    {
        var requests = await _repo.TeamJoinRequest.GetRequestsByTeamIdAsync(teamId, track);
        var requestsDto = _mapper.Map<IEnumerable<TeamJoinRequestDto>>(requests);

        foreach (var requestDto in requestsDto)
        {
            var photosDto = await _photoService.
                GetPhotosAsync("user", requestDto.UserId, false);

            var photoUrl = photosDto?.FirstOrDefault()?.Url ?? "";
            requestDto.UserPhotoUrl = photoUrl;
        }

        return requestsDto;
    }

    public async Task<IEnumerable<TeamJoinRequestDto>> GetUserJoinRequestsAsync(int userId, bool track)
    {
        var requests = await _repo.TeamJoinRequest.GetRequestsByUserIdAsync(userId, track);
        var requestsDto = _mapper.Map<IEnumerable<TeamJoinRequestDto>>(requests);
        return requestsDto;
    }

    public async Task<TeamJoinRequestDto> CreateJoinRequestAsync(int teamId, int userId)
    {
        var team = await CheckTeamExists(teamId);

        var pending = (await _repo.TeamJoinRequest
                           .GetPendingRequestsByTeamIdAsync(teamId, true))
                         .FirstOrDefault(r => r.UserId == userId);

        if (pending is not null)
            throw new InvalidOperationException("Bekleyen isteğiniz zaten var.");

        var req = new TeamJoinRequest { TeamId = teamId, UserId = userId };
        _repo.TeamJoinRequest.CreateJoinRequest(req);
        await _repo.SaveAsync();

        var reqDto = await _repo.TeamJoinRequest.GetJoinRequestAsync(req.Id, false);
        return _mapper.Map<TeamJoinRequestDto>(reqDto);
    }

    public async Task RespondJoinRequestAsync(int teamId, int requestId, TeamJoinRequestDtoForUpdate dto, int responderId)
    {
        var req = await CheckTeamJoinRequest(requestId);

        var member = await CheckTeamMembership(teamId, responderId);

        if (!member.IsCaptain)
            throw new UnauthorizedAccessException("Bu işlemi yalnızca takım kaptanları yapabilir.");

        req.Status = dto.Status;
        req.RespondedAt = DateTime.UtcNow;

        if (dto.Status == RequestStatus.Accepted)
        {
            var newMember = new TeamMember
            {
                TeamId = req.TeamId,
                UserId = req.UserId,
                IsCaptain = false,
                JoinedAt = DateTime.UtcNow
            };
            _repo.TeamMember.AddMember(newMember);
        }

        await _repo.SaveAsync();
    }

    private async Task<Team> CheckTeamExists(int teamId)
    {
        var entity = await _repo.Team.GetTeamAsync(teamId, true)
                     ?? throw new TeamNotFoundException(teamId);

        return entity;
    }

    private async Task<TeamMember> CheckTeamMembership(int teamId, int userId)
    {
        var member = await _repo.TeamMember.GetMemberAsync(teamId, userId, true)
             ?? throw new TeamMemberNotFoundException(teamId, userId);

        return member;
    }

    private async Task<TeamJoinRequest> CheckTeamJoinRequest(int requestId)
    {
        var req = await _repo.TeamJoinRequest
                           .GetJoinRequestAsync(requestId, trackChanges: true)
                 ?? throw new JoinRequestNotFoundException(requestId);

        return req;
    }

    private async Task SetUserStatus(TeamDto teamDto, int teamId, int reviewerId)
    {
        var teamMember = teamDto.Members.FirstOrDefault(m => m.UserId == reviewerId);

        if (teamMember is not null)
        {
            teamDto.UserStatus = (teamMember.IsCaptain, teamMember.IsAdmin) switch
            {
                (true, true) => UserStatus.CaptainAndAdmin,
                (true, false) => UserStatus.Captain,
                (false, true) => UserStatus.Admin,
                (false, false) => UserStatus.Member
            };
        }
        else
        {
            var joinRequests = await _repo.TeamJoinRequest.GetPendingRequestsByTeamIdAsync(teamId, false);
            var hasRequest = joinRequests.Any(r => r.UserId == reviewerId);

            if (hasRequest)
                teamDto.UserStatus = UserStatus.JoinRequestPending;
            else
                teamDto.UserStatus = UserStatus.NotMember;
        }
    }
}
