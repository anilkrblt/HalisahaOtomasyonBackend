using Entities.Models;
using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface IRoomService
{

    Task SetReadyAsync(int roomId, int teamId);
    Task RespondInviteAsync(int roomId, int teamId, int userId, bool accept);
    /*──────── ROOM CRUD ────────*/
    Task<RoomDto> CreateRoomAsync(RoomCreateDto dto, int creatorTeamId);
    Task<RoomDto?> GetRoomAsync(int roomId);
    Task<IEnumerable<RoomDto>> GetPublicRoomsAsync();

    /*──────── PARTICIPATION ────*/
    Task<RoomParticipantDto> JoinRoomAsync(int roomId, int teamId);          // public
    Task<RoomParticipantDto> JoinRoomByCodeAsync(string joinCode, int teamId);   // private

    /*──────── PAYMENT ─────────*/
    Task PayAsync(int roomId, int teamId, decimal amount);

    /*──────── MATCH START ─────*/
    Task<MatchDto> StartMatchAsync(int roomId, int startedByTeamId);

    /*──────── Opsiyonel eski Participant / Membership API’si ─────*/
    Task<IEnumerable<RoomParticipantDto>> GetParticipantsByRoomAsync(int roomId, bool trackChanges);
    Task<IEnumerable<RoomParticipantDto>> GetParticipantsByTeamAsync(int teamId, bool trackChanges);
    Task UpdateParticipantStatusAsync(int roomId, int teamId, ParticipantStatus status);
    Task RemoveParticipantAsync(int roomId, int teamId);

    Task<MonthlyMembershipDto> CreateMembershipAsync(MonthlyMembershipForCreationDto dto);
    Task<IEnumerable<MonthlyMembershipDto>> GetMembershipsByUserAsync(int userId, bool trackChanges);
    Task<IEnumerable<MonthlyMembershipDto>> GetAllMembershipsAsync(bool trackChanges);
    Task UpdateMembershipAsync(int id, MonthlyMembershipForUpdateDto dto);
    Task DeleteMembershipAsync(int id);
}
