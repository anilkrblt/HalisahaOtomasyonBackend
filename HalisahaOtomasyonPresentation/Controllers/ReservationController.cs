using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationController : ControllerBase
{
    private readonly IServiceManager _svc;
    public ReservationController(IServiceManager sm)
    {
        _svc = sm;
    }

    /*──────────────  RESERVATION CRUD  ──────────────*/


    // POST api/reservations/team
    [HttpPost("team")]
    public async Task<IActionResult> CreateTeamReservation(
          [FromBody] ReservationForCreationDto dto,
          [FromQuery] int homeTeamId,
          [FromQuery] int opponentTeamId,
          [FromQuery] int createdByUserId)
    {
        var created = await _svc.ReservationService.CreateTeamReservationAsync(dto, homeTeamId,
                                                            opponentTeamId, createdByUserId);
        return CreatedAtRoute("GetReservationById", new { id = created.Id }, created);
    }

    // PUT api/reservations/{id}/answer
    [HttpPut("{id:int}/answer")]
    public async Task<IActionResult> AnswerInvitation(
          int id,
          [FromQuery] int teamId,
          [FromQuery] bool accept)
    {
        await _svc.ReservationService.AnswerInvitationAsync(id, teamId, accept);
        return NoContent();
    }


    // GET api/reservations
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _svc.ReservationService.GetAllReservationsAsync(trackChanges: false));

    // GET api/reservations/42
    [HttpGet("{id:int}", Name = "GetReservationById")]
    public async Task<IActionResult> Get(int id)
        => Ok(await _svc.ReservationService.GetReservationAsync(id, trackChanges: false));

    // POST api/reservations
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReservationForCreationDto dto)
    {
        var created = await _svc.ReservationService.CreateReservationAsync(dto);
        return CreatedAtRoute("GetReservationById", new { id = created.Id }, created);
    }

    // PATCH api/reservations/42
    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id,
        [FromBody] ReservationForPatchUpdateDto patch)
    {
        await _svc.ReservationService.UpdateReservationAsync(id, patch);
        return NoContent();
    }

    // DELETE api/reservations/42
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.ReservationService.DeleteReservationAsync(id);
        return NoContent();
    }

    /*──────────────  FİLTRELİ LİSTELER  ──────────────*/

    // GET api/reservations/field/7
    [HttpGet("field/{fieldId:int}")]
    public async Task<IActionResult> GetByField(int fieldId)
        => Ok(await _svc.ReservationService.GetReservationsByFieldAsync(fieldId, false));

    // GET api/reservations/facility/3
    [HttpGet("facility/{facilityId:int}")]
    public async Task<IActionResult> GetByFacility(int facilityId)
        => Ok(await _svc.ReservationService.GetReservationsByFacilityAsync(facilityId, false));

    // GET api/reservations/user/15   (katılımcı)
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUser(int userId)
        => Ok(await _svc.ReservationService.GetReservationsByUserAsync(userId, false));
}
