using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IServiceManager _svc;
    public RoomsController(IServiceManager svc) => _svc = svc;


    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] RoomCreateDto dto, [FromQuery] int creatorTeamId)
    {
        var room = await _svc.RoomService.CreateRoomAsync(dto, creatorTeamId);
        return CreatedAtRoute("GetRoomById", new { id = room.RoomId }, room);
    }

    // GET api/rooms/42
    [HttpGet("{id:int}", Name = "GetRoomById")]
    public async Task<IActionResult> GetRoom(int id) =>
        Ok(await _svc.RoomService.GetRoomAsync(id));

    // POST api/rooms/42/ready?teamId=5
    [HttpPost("{id:int}/ready")]
    public async Task<IActionResult> SetReady(int id, [FromQuery] int teamId)
    {
        await _svc.RoomService.SetReadyAsync(id, teamId);
        return NoContent();
    }


    // POST api/rooms/42/invite/user/17
    [HttpPost("{roomId:int}/invite/user/{userId:int}")]
    public async Task<IActionResult> InviteUser(int roomId, int userId)
    {
        await _svc.RoomService.InviteUserToRoomAsync(roomId, userId);
        return NoContent();
    }


    // POST api/rooms/42/invite/respond?teamId=5&userId=17&accept=true
    // POST api/rooms/42/invite/user/17/respond?accept=true
    [HttpPost("{roomId:int}/invite/user/{userId:int}/respond")]
    public async Task<IActionResult> RespondUserInvite(int roomId, int userId, [FromQuery] bool accept)
    {
        await _svc.RoomService.RespondUserInviteAsync(roomId, userId, accept);
        return NoContent();
    }

    // GET api/rooms/42/participants
    [HttpGet("{id:int}/participants")]
    public async Task<IActionResult> GetParticipants(int id)
        => Ok(await _svc.RoomService.GetParticipantsByRoomAsync(id, false));
    // DELETE api/rooms/42/leave?teamId=5
    [HttpDelete("{id:int}/leave")]
    public async Task<IActionResult> LeaveRoom(int id, [FromQuery] int teamId)
    {
        await _svc.RoomService.RemoveParticipantAsync(id, teamId);
        return NoContent();
    }



    // GET api/rooms   (public lobby)
    [HttpGet]
    public async Task<IActionResult> GetPublicRooms() =>
        Ok(await _svc.RoomService.GetPublicRoomsAsync());

    /*──────── PARTICIPATION ────*/

    // POST api/rooms/42/join   (public)
    [HttpPost("{id:int}/join")]
    public async Task<IActionResult> JoinRoom(int id, [FromQuery] int teamId) =>
        Ok(await _svc.RoomService.JoinRoomAsync(id, teamId));

    // POST api/rooms/join?code=ABC123   (private)
    [HttpPost("join")]
    public async Task<IActionResult> JoinRoomByCode([FromQuery] string code,
                                                    [FromQuery] int teamId) =>
        Ok(await _svc.RoomService.JoinRoomByCodeAsync(code, teamId));

    /*──────── PAYMENT ─────────*/

    // POST api/rooms/42/pay
    [HttpPost("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id, [FromQuery] int teamId, [FromBody] PaymentDto dto)
    {
        await _svc.RoomService.PayAsync(id, teamId, dto.Amount);
        return NoContent();
    }

    /*──────── MATCH START ─────*/

    // POST api/rooms/42/start
    [HttpPost("{id:int}/start")]
    public async Task<IActionResult> StartMatch(int id, [FromQuery] int teamId) =>
        Ok(await _svc.RoomService.StartMatchAsync(id, teamId));
}
