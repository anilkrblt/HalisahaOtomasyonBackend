// Service/RoomService.cs
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class RoomService : IRoomService      
{
    private readonly IRepositoryManager _repo;
    private readonly INotificationService _notifs;
    private readonly ICodeGenerator _codeGen;
    private readonly IMapper _map;

    public RoomService(IRepositoryManager repo,
                       INotificationService notifs,
                       ICodeGenerator codeGen,
                       IMapper map)
    {
        _repo = repo;
        _notifs = notifs;
        _codeGen = codeGen;
        _map = map;
    }

    /*──────────────── ROOM CRUD ───────────────────────────────*/

    public async Task<RoomDto> CreateRoomAsync(RoomCreateDto dto, int creatorTeamId)
    {
        var field = await _repo.Field.GetFieldAsync(dto.FieldId, false) ?? throw new FieldNotFoundException(dto.FieldId);

        ValidateSlotAgainstField(dto.SlotStart, field);

        bool clash = (await _repo.Room.GetRoomsByFieldIdAsync(dto.FieldId, false))
                     .Any(r => r.SlotStart < dto.SlotStart.AddHours(1) && r.SlotEnd > dto.SlotStart);
        if (clash) throw new InvalidOperationException("Bu saat dolu.");

        var room = new Room
        {
            FieldId = dto.FieldId,
            SlotStart = dto.SlotStart,
            AccessType = dto.AccessType,
            JoinCode = dto.AccessType == RoomAccessType.Private ? _codeGen.Generate(6) : null,
            MaxPlayers = dto.MaxPlayers,
            PricePerPlayer = dto.PricePerPlayer ?? 0,
            Match = new Match()
        };

        _repo.Room.CreateRoom(room);
        await _repo.SaveAsync();

        _repo.RoomParticipant.CreateParticipant(new RoomParticipant
        {
            RoomId = room.Id,
            TeamId = creatorTeamId,
            IsHome = true,
            Status = ParticipantStatus.Accepted
        });
        await _repo.SaveAsync();

        return _map.Map<RoomDto>(room);
    }

    public async Task<RoomDto?> GetRoomAsync(int id) =>
        _map.Map<RoomDto>(await _repo.Room.GetOneRoomAsync(id, false));

    public async Task<IEnumerable<RoomDto>> GetPublicRoomsAsync() =>
        _map.Map<IEnumerable<RoomDto>>(
            await _repo.Room.GetPublicRoomsAsync(RoomAccessType.Public));

    /*──────────────── JOIN ───────────────────────────────────*/

    public async Task<RoomParticipantDto> JoinRoomAsync(int id, int teamId)
    {
        var room = await _repo.Room.GetOneRoomAsync(id, true)
                   ?? throw new RoomNotFoundException(id);

        if (room.AccessType != RoomAccessType.Public)
            throw new InvalidOperationException("Bu oda private.");

        return await InternalAddParticipant(room, teamId);
    }

    public async Task<RoomParticipantDto> JoinRoomByCodeAsync(string code, int teamId)
    {
        var room = await _repo.Room.GetRoomByCodeAsync(code, true)
                   ?? throw new ArgumentException("Kod hatalı.");

        return await InternalAddParticipant(room, teamId);
    }

    private async Task<RoomParticipantDto> InternalAddParticipant(Room room, int teamId)
    {
        if (room.Participants.Count >= room.MaxPlayers)
            throw new InvalidOperationException("Kapasite dolu.");

        var participant = new RoomParticipant
        {
            RoomId = room.Id,
            TeamId = teamId,
            Status = ParticipantStatus.Accepted
        };

        _repo.RoomParticipant.CreateParticipant(participant);
        await _repo.SaveAsync();

        await NotifyTeamAsync(teamId, "Odaya katıldınız.", room.Id);
        return _map.Map<RoomParticipantDto>(participant);
    }

    /*──────────────── PAYMENT ────────────────────────────────*/

    public async Task PayAsync(int roomId, int teamId, decimal amount)
    {
        var part = await _repo.RoomParticipant.GetParticipantAsync(roomId, teamId, true)
                   ?? throw new ParticipantNotFoundException(roomId, teamId);

       // if (part.Status.Unpaid) throw new InvalidOperationException("Zaten ödenmiş.");

        part.IsReady = true;
        part.PaidAmount = amount;
        await _repo.SaveAsync();
    }

    /*──────────────── MATCH START ────────────────────────────*/

    public async Task<MatchDto> StartMatchAsync(int roomId, int startedByTeamId)
    {
        var room = await _repo.Room.GetOneRoomAsync(roomId, true)
                   ?? throw new RoomNotFoundException(roomId);

        if (room.Status != RoomStatus.Confirmed)
            throw new InvalidOperationException("Oda henüz onaylı değil.");

        room.Status = RoomStatus.Played;
        room.Match.StartTime = room.SlotStart;
        room.Match.FieldId = room.FieldId;

        await _repo.SaveAsync();
        return _map.Map<MatchDto>(room.Match);
    }

    /*──────────────── Helpers ───────────────────────────────*/

    private static void ValidateSlotAgainstField(DateTime slotStart, Field field)
    {
        var open = field.WeeklyOpenings
                    .FirstOrDefault(o => o.DayOfWeek == slotStart.DayOfWeek)
                  ?? throw new InvalidOperationException("Saha o gün kapalı.");

        var st = slotStart.TimeOfDay;
        var et = st.Add(TimeSpan.FromHours(1));

        if (st < open.StartTime || et > open.EndTime)
            throw new InvalidOperationException(
                $"Saha {open.StartTime}-{open.EndTime} arası açık.");
    }

    private async Task NotifyTeamAsync(int teamId, string msg, int roomId)
    {
        var members = await _repo.TeamMember.GetMembersByTeamIdAsync(teamId, false);
        foreach (var m in members)
            await _notifs.CreateNotificationAsync(new NotificationForCreationDto
            {
                UserId = m.UserId,
                Title = "Oda",
                Content = msg,
                RelatedId = roomId,
                RelatedType = "room"
            });
    }

    public Task<IEnumerable<RoomParticipantDto>> GetParticipantsByRoomAsync(int roomId, bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RoomParticipantDto>> GetParticipantsByTeamAsync(int teamId, bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public Task UpdateParticipantStatusAsync(int roomId, int teamId, ParticipantStatus status)
    {
        throw new NotImplementedException();
    }

    public Task RemoveParticipantAsync(int roomId, int teamId)
    {
        throw new NotImplementedException();
    }

    public Task<MonthlyMembershipDto> CreateMembershipAsync(MonthlyMembershipForCreationDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MonthlyMembershipDto>> GetMembershipsByUserAsync(int userId, bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<MonthlyMembershipDto>> GetAllMembershipsAsync(bool trackChanges)
    {
        throw new NotImplementedException();
    }

    public Task UpdateMembershipAsync(int id, MonthlyMembershipForUpdateDto dto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMembershipAsync(int id)
    {
        throw new NotImplementedException();
    }

    /*──────────────── OPTIONAL eski membership / participant API ─────────*/
    // … aynı kalabilir, tip adlarını Room/RoomParticipant olarak değiştir.
}
    