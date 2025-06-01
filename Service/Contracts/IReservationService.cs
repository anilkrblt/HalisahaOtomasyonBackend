// Service.Contracts/IReservationService.cs
using Entities.Models;
using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface IReservationService
{
    /*────────  Reservation  ────────*/
    Task<ReservationDto> CreateReservationAsync(ReservationForCreationDto dto);
    Task<IEnumerable<ReservationDto>> GetAllReservationsAsync(bool trackChanges);
    Task<ReservationDto?> GetReservationAsync(int id, bool trackChanges);
    Task<IEnumerable<ReservationDto>> GetReservationsByFieldAsync(int fieldId, bool trackChanges);
    Task<IEnumerable<ReservationDto>> GetReservationsByFacilityAsync(int facilityId, bool trackChanges);
    Task<IEnumerable<ReservationDto>> GetReservationsByUserAsync(int userId, bool trackChanges);
    Task UpdateReservationAsync(int id, ReservationForPatchUpdateDto dto);
    Task DeleteReservationAsync(int id);

    /*────────  ReservationParticipant  ────────*/
    Task<ReservationParticipantDto> AddParticipantAsync(ReservationParticipantForCreationDto dto);
    Task<IEnumerable<ReservationParticipantDto>> GetParticipantsByReservationAsync(int reservationId, bool trackChanges);
    Task<IEnumerable<ReservationParticipantDto>> GetParticipantsByTeamAsync(int teamId, bool trackChanges);
    Task UpdateParticipantStatusAsync(int reservationId, int teamId, ParticipantStatus status);
    Task RemoveParticipantAsync(int reservationId, int teamId);

    /*────────  MonthlyMembership  ────────*/
    Task<MonthlyMembershipDto> CreateMembershipAsync(MonthlyMembershipForCreationDto dto);
    Task<IEnumerable<MonthlyMembershipDto>> GetMembershipsByUserAsync(int userId, bool trackChanges);
    Task<IEnumerable<MonthlyMembershipDto>> GetAllMembershipsAsync(bool trackChanges);
    Task UpdateMembershipAsync(int id, MonthlyMembershipForUpdateDto dto);
    Task DeleteMembershipAsync(int id);

    Task<ReservationDto> CreateTeamReservationAsync(
    ReservationForCreationDto slot,
    int homeTeamId,
    int opponentTeamId,
    int createdByUserId);

    Task AnswerInvitationAsync(        // rakip takımın cevabı
        int reservationId,
        int teamId,
        bool accept);
}
