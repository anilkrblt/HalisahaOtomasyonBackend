using Entities.Models;
using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface IRoomService
{
    Task RespondUserInviteAsync(int roomId, int userId, bool accept);
    Task InviteUserToRoomAsync(int roomId, int userId, int teamId);
    Task SetTeamReadyAsync(int roomId, int teamId, int userId);
    Task<RoomDto> CreateRoomAsync(RoomCreateDto dto, int creatorTeamId, int creatorUserId);

    Task<RoomDto?> GetRoomAsync(int roomId);
    Task<IEnumerable<RoomDto>> GetPublicRoomsAsync();
    Task InviteUsersToRoomAsync(int roomId, int teamId, List<int> userIds);
    Task<RoomParticipantDto> JoinRoomAsync(int id, int teamId, int userId);
    Task<RoomParticipantDto> JoinRoomAsAwayTeamAsync(int roomId, int teamId, int userId);
    Task StartPaymentPhaseAsync(int roomId, int userId);


    /*──────── PARTICIPATION ────*/
    //  Task<RoomParticipantDto> JoinRoomAsync(int roomId, int teamId);          // public
    Task<RoomParticipantDto> JoinRoomByCodeAsync(string joinCode, int teamId, int userId);

    /*──────── PAYMENT ─────────*/
    Task PayPlayerAsync(int roomId, int userId, decimal amount);
    Task ConfirmReservationAsync(int roomId);
    Task<(string ChargeId, decimal Amount)?> GetPaymentInfo(int roomId, int userId);
    Task<object> GetPaymentStatusAsync(int roomId); // dönüş tipi ihtiyaca göre değişebilir
    Task<IEnumerable<ReservationPaymentReportDto>> GetPaymentsByFieldOwnerAsync(int ownerId);

    Task ToggleUserReadyAsync(int roomId, int userId);

    Task<IEnumerable<RoomDto>> GetRoomsUserIsInvitedToAsync(int userId);


    Task PayAsync(int roomId, int teamId, decimal amount);

    /*──────── MATCH START ─────*/
    Task<MatchDto> StartMatchAsync(int roomId, int startedByTeamId);

    /*──────── Opsiyonel eski Participant / Membership API’si ─────*/
    Task<RoomParticipantsGroupedDto> GetParticipantsByRoomAsync(int roomId, bool trackChanges);
    Task<IEnumerable<RoomParticipantDto>> GetParticipantsByTeamAsync(int teamId, bool trackChanges);
    Task UpdateParticipantStatusAsync(int roomId, int teamId, ParticipantStatus status);
    Task RemoveParticipantAsync(int roomId, int teamId);

    Task<MonthlyMembershipDto> CreateMembershipAsync(MonthlyMembershipForCreationDto dto);
    Task<IEnumerable<MonthlyMembershipDto>> GetMembershipsByUserAsync(int userId, bool trackChanges);
    Task<IEnumerable<MonthlyMembershipDto>> GetAllMembershipsAsync(bool trackChanges);
    Task UpdateMembershipAsync(int id, MonthlyMembershipForUpdateDto dto);
    Task DeleteMembershipAsync(int id);
}
