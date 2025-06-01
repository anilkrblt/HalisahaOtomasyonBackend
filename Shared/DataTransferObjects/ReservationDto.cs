using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Shared.DataTransferObjects
{

    public record ReservationDto(
       int Id,
       int FieldId,
       DateTime SlotStart,
       DateTime SlotEnd,
       ReservationStatus Status,
       DateTime CreatedAt);

    public record ReservationForCreationDto(
      int FieldId,
      DateTime SlotStart);

    public record ReservationForUpdateDto(DateTime ReservationTime);

    public record ReservationForPatchUpdateDto(
        DateTime? SlotStart,
        ReservationStatus? Status);


    /*────────  ReservationParticipant  ────────*/
    public record ReservationParticipantDto(
        int ReservationId,
        int TeamId,
        bool IsHome,
        ParticipantStatus Status,
        DateTime? RespondedAt);

    public record ReservationParticipantForCreationDto(
        int ReservationId,
        int TeamId,
        bool IsHome);

    /*────────  MonthlyMembership  ────────*/
    public record MonthlyMembershipDto(
        int Id,
        int FieldId,
        int UserId,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        DateTime ExpirationDate);

    public record MonthlyMembershipForCreationDto(
        int FieldId,
        int UserId,
        DateTime ExpirationDate);

    public record MonthlyMembershipForUpdateDto(
        DateTime? ExpirationDate);

}