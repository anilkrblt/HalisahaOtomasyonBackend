using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class ReservationService : IReservationService
{
    private readonly IRepositoryManager _repo;
    private readonly INotificationService _notifs;

    private readonly IMapper _map;

    public ReservationService(IRepositoryManager repo,
     INotificationService notifs,
     IMapper map)
    {
        _repo = repo;
        _map = map;
        _notifs = notifs;
    }

    /*────────────────────  CREATE  ────────────────────*/
    public async Task<ReservationDto> CreateReservationAsync(ReservationForCreationDto dto)
    {
        /* 1) Saha var mı? */
        var field = await _repo.Field.GetFieldAsync(dto.FieldId, false)
                    ?? throw new FieldNotFoundException(dto.FieldId);

        /* 2) Açılış gün-saat kontrolü */
        ValidateSlotAgainstField(dto.SlotStart, field);

        /* 3) Çakışma kontrolü */
        var existing = await _repo.Reservation
            .GetReservationsByFieldIdAsync(dto.FieldId, false);

        bool conflict = existing.Any(r =>
             r.SlotStart < dto.SlotStart.AddHours(1) &&
             r.SlotEnd > dto.SlotStart);

        if (conflict)
            throw new InvalidOperationException("Bu saat için zaten rezervasyon var.");

        /* 4) Kaydet */
        var entity = _map.Map<Reservation>(dto);          // SlotStart, FieldId
        _repo.Reservation.CreateReservation(entity);
        await _repo.SaveAsync();

        return _map.Map<ReservationDto>(entity);
    }

    /*────────────────────  READ  ──────────────────────*/
    public async Task<IEnumerable<ReservationDto>> GetAllReservationsAsync(bool track) =>
        _map.Map<IEnumerable<ReservationDto>>(
            await _repo.Reservation.GetAllReservationsAsync(track));

    public async Task<ReservationDto?> GetReservationAsync(int id, bool track) =>
        _map.Map<ReservationDto>(
            await _repo.Reservation.GetOneReservationAsync(id, track));

    public async Task<IEnumerable<ReservationDto>>GetReservationsByFieldAsync(int fieldId, bool track) =>
        _map.Map<IEnumerable<ReservationDto>>(
            await _repo.Reservation.GetReservationsByFieldIdAsync(fieldId, track));

    public async Task<IEnumerable<ReservationDto>>
        GetReservationsByFacilityAsync(int facId, bool track) =>
        _map.Map<IEnumerable<ReservationDto>>(
            await _repo.Reservation.GetReservationsByFacilityIdAsync(facId, track));

    public async Task<IEnumerable<ReservationDto>>
        GetReservationsByUserAsync(int userId, bool track) =>
        _map.Map<IEnumerable<ReservationDto>>(
            await _repo.Reservation.GetReservationsByUserIdAsync(userId, track));

    /*────────────────────  UPDATE  ────────────────────*/
    public async Task UpdateReservationAsync(int id,
        ReservationForPatchUpdateDto patch)
    {
        var res = await _repo.Reservation.GetOneReservationAsync(id, true)
                  ?? throw new ReservationNotFoundException(id);

        if (patch.SlotStart.HasValue)
        {
            var field = await _repo.Field.GetFieldAsync(res.FieldId, false)!
                       ?? throw new FieldNotFoundException(res.FieldId);

            ValidateSlotAgainstField(patch.SlotStart.Value, field);

            /* çakışma? */
            var existing = await _repo.Reservation
                .GetReservationsByFieldIdAsync(res.FieldId, false);

            bool conflict = existing.Any(r => r.Id != id &&
                   r.SlotStart < patch.SlotStart.Value.AddHours(1) &&
                   r.SlotEnd > patch.SlotStart.Value);

            if (conflict)
                throw new InvalidOperationException("Bu saat için başka rezervasyon var.");

            res.SlotStart = patch.SlotStart.Value;
        }

        if (patch.Status.HasValue)
            res.Status = patch.Status.Value;

        await _repo.SaveAsync();
    }

    /*────────────────────  DELETE  ────────────────────*/
    public async Task DeleteReservationAsync(int id)
    {
        var res = await _repo.Reservation.GetOneReservationAsync(id, true)
                  ?? throw new ReservationNotFoundException(id);

        _repo.Reservation.DeleteReservation(res);
        await _repo.SaveAsync();
    }




    /*──────────── 1) TAKIM REZERVASYONU OLUŞTUR ────────────*/
    public async Task<ReservationDto> CreateTeamReservationAsync(
        ReservationForCreationDto slot,
        int homeTeamId,
        int opponentTeamId,
        int createdByUserId)
    {
        /*—1) Alan & zaman doğrulamaları—*/
        var field = await _repo.Field.GetFieldAsync(slot.FieldId, false)
                    ?? throw new FieldNotFoundException(slot.FieldId);

        ValidateSlotAgainstField(slot.SlotStart, field);

        var overlaps = (await _repo.Reservation
                            .GetReservationsByFieldIdAsync(slot.FieldId, false))
                       .Any(r => r.SlotStart < slot.SlotStart.AddHours(1) &&
                                 r.SlotEnd > slot.SlotStart);
        if (overlaps) throw new InvalidOperationException("Saat çakışıyor.");

        /*—2) Rezervasyonu & katılımcıları oluştur—*/
        var reservation = new Reservation
        {
            FieldId = slot.FieldId,
            SlotStart = slot.SlotStart,
            Status = ReservationStatus.PendingOpponent
        };

        _repo.Reservation.CreateReservation(reservation);
        await _repo.SaveAsync();            // id üretsin

        /* Home takım – otomatik ‘Accepted’                          */
        _repo.ReservationParticipant.CreateParticipant(new ReservationParticipant
        {
            ReservationId = reservation.Id,
            TeamId = homeTeamId,
            IsHome = true,
            Status = ParticipantStatus.Accepted
        });

        /* Rakip takım – ‘Invited’                                   */
        _repo.ReservationParticipant.CreateParticipant(new ReservationParticipant
        {
            ReservationId = reservation.Id,
            TeamId = opponentTeamId,
            IsHome = false,
            Status = ParticipantStatus.Invited
        });

        await _repo.SaveAsync();

        /*—3) Rakip takım üyelerine davet bildirimi—*/
        await NotifyTeamAsync(opponentTeamId,
            $"Takımınız {slot.SlotStart:dd.MM HH:mm} slotu için davet aldı.",
            reservation.Id);

        return _map.Map<ReservationDto>(reservation);
    }

    /*──────────── 2) DAVETE YANIT VER ────────────*/
    public async Task AnswerInvitationAsync(int reservationId, int teamId, bool accept)
    {
        /* Katılımcı satırını çek */
        var participant = await _repo.ReservationParticipant
            .GetParticipantAsync(reservationId, teamId, true)
            ?? throw new ParticipantNotFoundException(reservationId, teamId);

        if (participant.Status != ParticipantStatus.Invited)
            throw new InvalidOperationException("Davet zaten yanıtlanmış.");

        participant.Status = accept ? ParticipantStatus.Accepted
                                        : ParticipantStatus.Rejected;
        participant.RespondedAt = DateTime.UtcNow;

        /* Rezervasyonu getir (diğer takımın cevabını görebilmek için) */
        var reservation = await _repo.Reservation.GetOneReservationAsync(reservationId, true)!;

        /* Her iki takım da kabul etti mi?  -> Confirmed */
        bool bothAccepted = reservation.Participants
            .All(p => p.Status == ParticipantStatus.Accepted);

        reservation.Status = accept
            ? bothAccepted ? ReservationStatus.Confirmed
                           : ReservationStatus.WaitingConfirm
            : ReservationStatus.Cancelled;

        await _repo.SaveAsync();

        /* Bildirimler */
        if (accept)
        {
            // kabul eden takım onay bildirimi
            await NotifyTeamAsync(teamId,
                "Rezervasyon davetini kabul ettiniz.", reservationId);

            if (bothAccepted)
            {
                // Her iki tarafa da maç kesinleşti bildirimi
                foreach (var tId in reservation.Participants.Select(p => p.TeamId))
                    await NotifyTeamAsync(tId,
                        "Maç rezervasyonu karşılıklı onaylandı!", reservationId);
            }
        }
        else
        {
            // reddedildi -> diğer takıma iptal bildirimi
            var other = reservation.Participants.First(p => p.TeamId != teamId).TeamId;
            await NotifyTeamAsync(other,
                "Rakip takım davetinizi reddetti.", reservationId);
        }
    }


    private async Task NotifyTeamAsync(int teamId, string msg, int reservationId)
    {
        /* Takımdaki tüm kullanıcı id’lerini bul */
        var members = await _repo.TeamMember.GetMembersByTeamIdAsync(teamId, false);
        foreach (var m in members)
        {
            await _notifs.CreateNotificationAsync(new NotificationForCreationDto
            {
                UserId = m.UserId,
                Title = "Rezervasyon",
                Content = msg,
                RelatedId = reservationId,
                RelatedType = "reservation"
            });
        }
    }



    /*────────── Yardımcı: saha açık mı? ──────────*/
    private static void ValidateSlotAgainstField(DateTime slotStart, Field field)
    {
        var open = field.WeeklyOpenings
                    .FirstOrDefault(w => w.DayOfWeek == slotStart.DayOfWeek);

        if (open == null)
            throw new InvalidOperationException("Saha o gün kapalı.");

        var start = slotStart.TimeOfDay;
        var end = start.Add(TimeSpan.FromHours(1));

        if (start < open.StartTime || end > open.EndTime)
            throw new InvalidOperationException(
                $"Saha {open.StartTime}-{open.EndTime} arası açık.");
    }







    /*───────────────────────────────────────────────*/
    /*  ReservationParticipant & MonthlyMembership  */
    /*  metodları (daha önceki yanıtta) aynen burada */

    /*────────────── PARTICIPANT ──────────────*/

    public async Task<ReservationParticipantDto>
        AddParticipantAsync(ReservationParticipantForCreationDto dto)
    {
        var entity = _map.Map<ReservationParticipant>(dto);
        _repo.ReservationParticipant.CreateParticipant(entity);
        await _repo.SaveAsync();
        return _map.Map<ReservationParticipantDto>(entity);
    }

    public async Task<IEnumerable<ReservationParticipantDto>>
        GetParticipantsByReservationAsync(int resId, bool t) =>
        _map.Map<IEnumerable<ReservationParticipantDto>>(
            await _repo.ReservationParticipant.GetParticipantsByReservationIdAsync(resId, t));

    public async Task<IEnumerable<ReservationParticipantDto>>
        GetParticipantsByTeamAsync(int teamId, bool t) =>
        _map.Map<IEnumerable<ReservationParticipantDto>>(
            await _repo.ReservationParticipant.GetParticipantsByTeamIdAsync(teamId, t));

    public async Task UpdateParticipantStatusAsync(int resId, int teamId, ParticipantStatus status)
    {
        var row = await _repo.ReservationParticipant.GetParticipantAsync(resId, teamId, true)
                  ?? throw new ParticipantNotFoundException(resId, teamId);

        row.Status = status;
        row.RespondedAt = DateTime.UtcNow;
        await _repo.SaveAsync();
    }

    public async Task RemoveParticipantAsync(int resId, int teamId)
    {
        var row = await _repo.ReservationParticipant.GetParticipantAsync(resId, teamId, true)
                  ?? throw new ParticipantNotFoundException(resId, teamId);

        _repo.ReservationParticipant.DeleteParticipant(row);
        await _repo.SaveAsync();
    }

    /*────────────── MEMBERSHIP ──────────────*/

    public async Task<MonthlyMembershipDto> CreateMembershipAsync(MonthlyMembershipForCreationDto dto)
    {
        var entity = _map.Map<MonthlyMembership>(dto);
        _repo.MonthlyMembership.CreateMonthlyMembership(entity);
        await _repo.SaveAsync();
        return _map.Map<MonthlyMembershipDto>(entity);
    }

    public async Task<IEnumerable<MonthlyMembershipDto>> GetMembershipsByUserAsync(int uId, bool t) =>
        _map.Map<IEnumerable<MonthlyMembershipDto>>(
            await _repo.MonthlyMembership.GetMonthlyMembershipsByCustomerIdAsync(uId, t));

    public async Task<IEnumerable<MonthlyMembershipDto>> GetAllMembershipsAsync(bool t) =>
        _map.Map<IEnumerable<MonthlyMembershipDto>>(
            await _repo.MonthlyMembership.GetAllMonthlyMembershipsAsync(t));

    public async Task UpdateMembershipAsync(int id, MonthlyMembershipForUpdateDto dto)
    {
        var entity = await _repo.MonthlyMembership.GetOneMonthlyMembershipAsync(id, true)
                     ?? throw new MembershipNotFoundException(id);

        if (dto.ExpirationDate.HasValue) entity.ExpirationDate = dto.ExpirationDate.Value;
        entity.UpdatedAt = DateTime.UtcNow;
        await _repo.SaveAsync();
    }

    public async Task DeleteMembershipAsync(int id)
    {
        var entity = await _repo.MonthlyMembership.GetOneMonthlyMembershipAsync(id, true)
                     ?? throw new MembershipNotFoundException(id);

        _repo.MonthlyMembership.DeleteMonthlyMembership(entity);
        await _repo.SaveAsync();
    }

}





















/* Basit istisna örnekleri */
public sealed class ReservationNotFoundException : NotFoundException
{ public ReservationNotFoundException(int id) : base($"Reservation ({id}) not found.") { } }

public sealed class ParticipantNotFoundException : NotFoundException
{ public ParticipantNotFoundException(int r, int t) : base($"Participant R{r}-T{t} not found.") { } }

public sealed class MembershipNotFoundException : NotFoundException
{ public MembershipNotFoundException(int id) : base($"Membership ({id}) not found.") { } }

