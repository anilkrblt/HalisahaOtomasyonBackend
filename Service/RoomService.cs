// Service/RoomService.cs
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class RoomService : IRoomService
{
    private readonly IRepositoryManager _repo;
    private readonly INotificationService _notifs;
    private readonly ICodeGenerator _codeGen;
    private readonly IMapper _map;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoomService(IRepositoryManager repo,
                       INotificationService notifs,
                       ICodeGenerator codeGen,
                       IMapper map,
                       UserManager<ApplicationUser> userManager)
    {
        _repo = repo;
        _notifs = notifs;
        _codeGen = codeGen;
        _map = map;
        _userManager = userManager;
    }

    /*──────────────── ROOM CRUD ───────────────────────────────*/

    public async Task<RoomDto> CreateRoomAsync(RoomCreateDto dto, int creatorTeamId)
    {
        var field = await _repo.Field.GetFieldAsync(dto.FieldId, false) ?? throw new FieldNotFoundException(dto.FieldId);

        ValidateSlotAgainstField(dto.SlotStart, field);

        bool clash = (await _repo.Room.GetRoomsByFieldIdAsync(dto.FieldId, false))
                     .Any(r => r.SlotStart < dto.SlotStart.AddHours(1) && r.SlotEnd > dto.SlotStart);
        bool clash2 = (await _repo.Reservation.GetByFieldAsync(dto.FieldId, dto.SlotStart)).Any();
        if (clash || clash2) throw new InvalidOperationException("Bu saat dolu.");


        var room = new Room
        {
            FieldId = dto.FieldId,
            SlotStart = dto.SlotStart,
            AccessType = dto.AccessType,
            JoinCode = dto.AccessType == RoomAccessType.Private ? _codeGen.Generate(6) : null,
            MaxPlayers = dto.MaxPlayers,
            PricePerPlayer = field.PricePerHour / (dto.MaxPlayers == 0 ? 1 : dto.MaxPlayers)
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


    public async Task InviteUserToRoomAsync(int roomId, int userId)
    {
        var room = await _repo.Room.GetOneRoomAsync(roomId, false)
                   ?? throw new RoomNotFoundException(roomId);

        var user = await _userManager.FindByIdAsync(userId.ToString())
                    ?? throw new UserNotFoundException(userId);

        // Aynı kullanıcı zaten bu odada mı?
        var existing = await _repo.RoomParticipant.GetByCustomerAsync(roomId, userId);
        if (existing is not null)
            throw new InvalidOperationException("Bu kullanıcı zaten bu odada.");

        var participant = new RoomParticipant
        {
            RoomId = roomId,
            CustomerId = userId,
            Status = ParticipantStatus.Invited
        };

        _repo.RoomParticipant.CreateParticipant(participant);
        await _repo.SaveAsync();

        await _notifs.CreateNotificationAsync(new NotificationForCreationDto
        {
            UserId = userId,
            Title = "Maç Daveti",
            Content = $"Bir maça davet edildin! Oda ID: #{roomId}",
            RelatedId = roomId,
            RelatedType = "room"
        });
    }


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
        // 1. Aynı anahtara sahip bir RoomParticipant zaten var mı? Kontrol et:
        var existing = await _repo.RoomParticipant.GetParticipantAsync(room.Id, teamId, true);
        if (existing != null)
            throw new InvalidOperationException("Bu takım zaten bu odada.");

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

        await NotifyTeamInviteAsync(teamId, room.Id);



        if (room.Participants.Count + 1 >= 2)
        {
            room.Status = RoomStatus.WaitingConfirm;
            await _repo.SaveAsync();
        }

        await NotifyTeamAsync(teamId, "Odaya katıldınız.", room.Id);
        return _map.Map<RoomParticipantDto>(participant);
    }


    /*──────────────── PAYMENT ────────────────────────────────*/

    public async Task PayAsync(int roomId, int teamId, decimal amount)
    {
        var part = await _repo.RoomParticipant.GetParticipantAsync(roomId, teamId, true)
                    ?? throw new ParticipantNotFoundException(roomId, teamId);

        if (part.PaymentStatus == PaymentStatus.Paid)
            throw new InvalidOperationException("Zaten ödenmiş.");

        part.PaymentStatus = PaymentStatus.Paid;
        part.PaidAmount = amount;
        part.IsReady = true;

        // ReservationPayment kaydı
        _repo.ReservationPayment.CreateReservationPayment(new ReservationPayment
        {
            ReservationId = part.Room!.Reservation!.Id,   // Reservation lobby onaylandığında oluşturulmuş olmalı
            Amount = amount,
            Status = PaymentStatus.Paid,
            RoomParticipantRoomId = roomId,
            RoomParticipantTeamId = teamId,
            PaidAt = DateTime.UtcNow
        });

        await _repo.SaveAsync();

        // tüm katılımcılar ödedi mi?
        var allPaid = part.Room.Participants.All(p => p.PaymentStatus == PaymentStatus.Paid);
        if (allPaid)
        {
            part.Room.Reservation!.Status = ReservationStatus.Confirmed;
            part.Room.Status = RoomStatus.Confirmed;
            await _repo.SaveAsync();
        }
    }


    /*──────────────── MATCH START ────────────────────────────*/

    public async Task<MatchDto> StartMatchAsync(int roomId, int startedByTeamId)
    {
        var room = await _repo.Room.GetOneRoomAsync(roomId, true)
                     ?? throw new RoomNotFoundException(roomId);

        if (room.Status != RoomStatus.Confirmed ||
            room.Reservation?.Status != ReservationStatus.Confirmed)
            throw new InvalidOperationException("Oda veya rezervasyon henüz onaylı değil.");

        var isStarterParticipant = room.Participants.Any(p => p.TeamId == startedByTeamId);
        if (!isStarterParticipant)
            throw new UnauthorizedAccessException("Bu takım maçı başlatamaz.");

        if (room.Match is null)
        {
            var homeId = room.Participants.FirstOrDefault(p => p.IsHome)?.TeamId;
            var awayId = room.Participants.FirstOrDefault(p => !p.IsHome)?.TeamId;
            if (homeId is null || awayId is null)
                throw new InvalidOperationException("Home/Away takımları atanamadı.");

            var match = new Match
            {
                RoomId = room.Id,
                StartTime = room.SlotStart,
                FieldId = room.FieldId,
                HomeTeamId = homeId,
                AwayTeamId = awayId
            };
            _repo.Match.CreateMatch(match);
            room.Match = match;
        }

        room.Status = RoomStatus.Played;
        room.Reservation!.Status = ReservationStatus.Played;

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
        var members = await _repo.TeamMember.GetMembersByTeamIdWithUserAsync(teamId, false);
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


    private async Task NotifyTeamInviteAsync(int teamId, int roomId)
    {
        var members = await _repo.TeamMember.GetMembersByTeamIdWithUserAsync(teamId, false);
        foreach (var m in members)
            await _notifs.CreateNotificationAsync(new NotificationForCreationDto
            {
                UserId = m.UserId,
                Title = "Maç Daveti",
                Content = $"Takımın şu odaya katıldı: #{roomId}. Katılımını onayla!",
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

    public async Task SetReadyAsync(int roomId, int teamId)
    {
        var part = await _repo.RoomParticipant.GetParticipantAsync(roomId, teamId, true)
                  ?? throw new Exception("Katılımcı bulunamadı.");

        part.IsReady = true;
        await _repo.SaveAsync();
    }
    public async Task RespondUserInviteAsync(int roomId, int userId, bool accept)
    {
        var participant = await _repo.RoomParticipant.GetByCustomerAsync(roomId, userId)
                          ?? throw new Exception("Kullanıcı bu odaya davetli değil.");

        participant.Status = accept ? ParticipantStatus.Accepted : ParticipantStatus.Rejected;
        await _repo.SaveAsync();
    }


}
