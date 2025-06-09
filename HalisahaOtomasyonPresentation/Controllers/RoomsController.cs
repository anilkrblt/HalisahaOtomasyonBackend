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
    public async Task<IActionResult> CreateRoom([FromBody] RoomCreateDto dto,
                                                [FromQuery] int creatorTeamId)
    {
        var room = await _svc.RoomService.CreateRoomAsync(dto, creatorTeamId);
        return CreatedAtRoute("GetRoomById", new { id = room.RoomId }, room);
    }

    // GET api/rooms/42
    [HttpGet("{id:int}", Name = "GetRoomById")]
    public async Task<IActionResult> GetRoom(int id) =>
        Ok(await _svc.RoomService.GetRoomAsync(id));

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
    public async Task<IActionResult> Pay(int id,
                                         [FromQuery] int teamId,
                                         [FromBody] PaymentDto dto)
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
