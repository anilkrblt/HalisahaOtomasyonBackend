using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Service.Contracts;
using Shared.DataTransferObjects;

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

    /*────────────────────  TEAM  ────────────────────*/

    // Service/TeamService.cs
    public async Task<TeamDto> CreateTeamAsync(TeamForCreationDto dto, int creatorUserId)
    {
        // 1) Takımı oluştur
        var entity = _mapper.Map<Team>(dto);
        _repo.Team.CreateTeam(entity);
        await _repo.SaveAsync(); // entity.Id oluştu

        // 2) Oluşturan kullanıcıyı kaptan üye olarak ekle
        var captain = new TeamMember
        {
            TeamId = entity.Id,
            UserId = creatorUserId,
            IsCaptain = true,
            IsAdmin = true,
            Position = PlayerPosition.Utility,  // istersen DTO’ya default pozisyon ekleyebilirsin
            JoinedAt = DateTime.UtcNow
        };
        _repo.TeamMember.AddMember(captain);
        await _repo.SaveAsync();

        // 3) Son hali tekrar yükleyip DTO’ya çevir
        var fullTeam = await _repo.Team.GetTeamAsync(entity.Id, trackChanges: false);
        return _mapper.Map<TeamDto>(fullTeam);
    }


    public async Task SetTeamLogoAsync(int teamId, IFormFile logoFile)
    {
        var team = await _repo.Team.GetTeamAsync(teamId, trackChanges: true)
                   ?? throw new TeamNotFoundException(teamId);

        // logosu “team/{id}” klasörüne koy, path’i al
        var url = await _photoService.UploadLogoAsync(logoFile, $"team/{teamId}");
        team.LogoUrl = url;
        await _repo.SaveAsync();
    }

    public async Task<TeamDto> GetTeamAsync(int teamId, bool track)
    {
        var teamEntity = await _repo.Team.GetTeamAsync(teamId, track)
                         ?? throw new TeamNotFoundException(teamId);

        // TeamMember + User bilgileriyle beraber geliyor olmalı
        var members = await _repo.TeamMember.GetMembersByTeamIdAsync(teamId, trackChanges: false);

        // Takım DTO’sunu mapleyelim
        var teamDto = _mapper.Map<TeamDto>(teamEntity);

        // 🔧 TeamMemberDto'ları manuel olarak oluşturalım
        var memberDtos = new List<TeamMemberDto>();

        Console.WriteLine($"Team ID: {teamId} için {members.Count()} adet member bulundu.");

        foreach (var member in members)
        {
            var user = member.User; // Include ile gelmeli
            var photoDto = await _photoService.GetPhotosAsync("user", member.UserId, false);

            var memberDto = new TeamMemberDto
            {
                UserId = member.UserId,
                UserName = user?.UserName ?? "",
                FirstName = user?.FirstName ?? "",
                LastName = user?.LastName ?? "",
                IsCaptain = member.IsCaptain,
                IsAdmin = member.IsAdmin,
                Position = member.Position,
                JoinedAt = member.JoinedAt,
                UserPhotoUrl = photoDto?.FirstOrDefault()?.Url ?? ""
            };

            memberDtos.Add(memberDto);
        }

        teamDto = teamDto with { Members = memberDtos };

        return teamDto;
    }

    public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync(bool track)
    {
        // 1. Tüm takımları al
        var teamEntities = await _repo.Team.GetAllTeamsAsync(track);
        var teamDtos = new List<TeamDto>();

        // 2. Her takım için üyeleri çek ve manuel olarak TeamDto oluştur
        foreach (var team in teamEntities)
        {
            var members = await _repo.TeamMember.GetMembersByTeamIdAsync(team.Id, trackChanges: false);

            var memberDtos = new List<TeamMemberDto>();

            foreach (var member in members)
            {
                var user = member.User; // Include ile gelmiş olmalı
                var photoDto = await _photoService.GetPhotosAsync("user", member.UserId, false);

                var memberDto = new TeamMemberDto
                {
                    UserId = member.UserId,
                    UserName = user?.UserName ?? "",
                    FirstName = user?.FirstName ?? "",
                    LastName = user?.LastName ?? "",
                    IsCaptain = member.IsCaptain,
                    IsAdmin = member.IsAdmin,
                    Position = member.Position,
                    JoinedAt = member.JoinedAt,
                    UserPhotoUrl = photoDto?.FirstOrDefault()?.Url ?? ""
                };

                memberDtos.Add(memberDto);
            }

            var teamDto = _mapper.Map<TeamDto>(team);
            teamDto = teamDto with { Members = memberDtos };

            teamDtos.Add(teamDto);
        }

        return teamDtos;
    }



    public async Task<IEnumerable<TeamDto>> GetTeamsByCityAsync(string city, bool track) =>
        _mapper.Map<IEnumerable<TeamDto>>(
            await _repo.Team.GetTeamsByCityAsync(city, track));

    public async Task<IEnumerable<TeamDto>> SearchTeamsByNameAsync(string keyword, bool track) =>
        _mapper.Map<IEnumerable<TeamDto>>(
            await _repo.Team.SearchTeamsByNameAsync(keyword, track));

    public async Task<TeamDto> UpdateTeamAsync(int id, TeamForUpdateDto dto)
    {
        var entity = await _repo.Team.GetTeamAsync(id, true)
                     ?? throw new TeamNotFoundException(id);

        _mapper.Map(dto, entity);
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

    /*─────────────────  TEAM MEMBERS  ───────────────*/

    public async Task AddMemberAsync(int teamId, int userId, PlayerPosition pos, bool isCaptain)
    {
        var team = await _repo.Team.GetTeamAsync(teamId, true)
                   ?? throw new TeamNotFoundException(teamId);

        var exists = await _repo.TeamMember.GetMemberAsync(teamId, userId, false);
        if (exists is not null)
            throw new InvalidOperationException("Üye zaten takımda.");

        var member = new TeamMember
        {
            TeamId = teamId,
            UserId = userId,
            Position = pos,
            IsCaptain = isCaptain
        };
        _repo.TeamMember.AddMember(member);
        await _repo.SaveAsync();
    }

    public async Task RemoveMemberAsync(int teamId, int userId)
    {
        var member = await _repo.TeamMember.GetMemberAsync(teamId, userId, true)
                     ?? throw new TeamMemberNotFoundException(teamId, userId);

        _repo.TeamMember.RemoveMember(member);
        await _repo.SaveAsync();
    }

    public async Task<IEnumerable<TeamMemberDto>> GetMembersAsync(int teamId, bool track) =>
        _mapper.Map<IEnumerable<TeamMemberDto>>(
            await _repo.TeamMember.GetMembersByTeamIdAsync(teamId, track));

    public async Task<IEnumerable<TeamDto>> GetTeamsOfUserAsync(int userId, bool track) =>
        _mapper.Map<IEnumerable<TeamDto>>(
            await _repo.TeamMember.GetTeamsByUserIdAsync(userId, track));

    /*───────────────  JOIN REQUESTS  ────────────────*/


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

    public async Task<TeamMemberDto> SetAdminAndCaptain(int teamId, int userId, TeamMemberDtoForUpdateAdminAndCaptain teamMemberDto)
    {
        var member = await _repo.
            TeamMember
            .GetMemberAsync(teamId, userId, true);

        var updatedMember = _mapper.Map(teamMemberDto, member);
        await _repo.SaveAsync();

        var updatedMembersDto = _mapper.Map<TeamMemberDto>(updatedMember);
        return updatedMembersDto;
    }
}
