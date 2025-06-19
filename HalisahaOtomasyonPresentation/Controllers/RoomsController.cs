using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;
// TODO odaları kontrol et

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IServiceManager _svc;
    public RoomsController(IServiceManager svc) => _svc = svc;


    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRoom([FromBody] RoomCreateDto dto, [FromQuery] int creatorTeamId)
    {
        int userId = int.Parse(User.FindFirst("id")!.Value);

        var room = await _svc.RoomService.CreateRoomAsync(dto, creatorTeamId, userId);
        return CreatedAtRoute("GetRoomById", new { id = room.RoomId }, room);
    }




    [HttpGet("{id:int}", Name = "GetRoomById")]
    [Authorize]
    public async Task<IActionResult> GetRoom(int id)
    {
        int userId = int.Parse(User.FindFirst("id")!.Value);
        var room = await _svc.RoomService.GetRoomAsync(id, userId);
        return Ok(room);
    }


    [HttpPost("{roomId:int}/ready/team/{teamId:int}")]
    public async Task<IActionResult> SetTeamReady(int roomId, int teamId)
    {
        int userId = int.Parse(User.FindFirst("id")!.Value);
        await _svc.RoomService.SetTeamReadyAsync(roomId, teamId, userId);
        return NoContent();
    }

    // oda ödeme faslına geçer
    [HttpPost("{id:int}/start-payment")]
    [Authorize]
    public async Task<IActionResult> StartPaymentPhase(int id)
    {
        int userId = int.Parse(User.FindFirst("id")!.Value);
        await _svc.RoomService.StartPaymentPhaseAsync(id, userId);
        return NoContent();
    }

    // odada kimlerin ödeyip ödemediğini ve ödediği miktarı gösteriyor
    [HttpGet("{roomId:int}/payment-status")]
    public async Task<IActionResult> GetPaymentStatus(int roomId)
    {
        return Ok(await _svc.RoomService.GetPaymentStatusAsync(roomId));
    }

    /*

        [HttpGet("{roomId:int}/payment-info")]
        [Authorize]
        public async Task<IActionResult> GetPaymentInfo(int roomId)
        {
            int userId = int.Parse(User.FindFirst("id")!.Value);
            var result = await _svc.RoomService.GetPaymentInfo(roomId, userId);
            return Ok(result);
        }

    */


    // POST api/rooms/42/invite/user/17
    [HttpPost("{roomId:int}/invite/user/{userId:int}")]
    public async Task<IActionResult> InviteUser(int roomId, int userId, [FromQuery] int teamId)
    {
        await _svc.RoomService.InviteUserToRoomAsync(roomId, userId, teamId);
        return NoContent();
    }



    [HttpPost("{roomId:int}/invite/users")]
    public async Task<IActionResult> InviteUsers(int roomId, [FromQuery] int teamId, [FromBody] List<int> userIds)
    {
        await _svc.RoomService.InviteUsersToRoomAsync(roomId, teamId, userIds);
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


    [HttpGet("{id:int}/participants")]
    public async Task<IActionResult> GetParticipants(int id)
    {
        var result = await _svc.RoomService.GetParticipantsByRoomAsync(id, false);
        return Ok(result);
    }



    // TODO ayrılma bugı silmiyor
    [HttpDelete("{roomId:int}/leave")]
    [Authorize]
    public async Task<IActionResult> LeaveRoom(int roomId)
    {
        int userId = int.Parse(User.FindFirst("id")!.Value);
        await _svc.RoomService.RemoveParticipantAsync(roomId, userId);
        return NoContent();
    }




    // GET api/rooms   (public lobby)
    [HttpGet]
    public async Task<IActionResult> GetPublicRooms() =>
        Ok(await _svc.RoomService.GetPublicRoomsAsync());

    /*──────── PARTICIPATION ────*/

    [HttpPost("{id:int}/join")]
    [Authorize]
    public async Task<IActionResult> JoinRoom(int id, [FromQuery] int teamId)
    {
        int userId = int.Parse(User.FindFirst("id")!.Value);
        var result = await _svc.RoomService.JoinRoomAsAwayTeamAsync(id, teamId, userId);
        return Ok(result);
    }



    // POST api/rooms/join?code=ABC123   (private)
    [HttpPost("join")]
    public async Task<IActionResult> JoinRoomByCode([FromQuery] string code, [FromQuery] int teamId)
    {
        int userId = int.Parse(User.FindFirst("id")!.Value); // 👈 userId tokenden
        var result = await _svc.RoomService.JoinRoomByCodeAsync(code, teamId, userId);
        return Ok(result);
    }


    /*──────── PAYMENT ─────────*/

    [HttpPost("{roomId:int}/ready/user/toggle")]
    [Authorize]
    public async Task<IActionResult> ToggleUserReady(int roomId)
    {
        var userId = int.Parse(User.FindFirst("id")!.Value);
        await _svc.RoomService.ToggleUserReadyAsync(roomId, userId);
        return NoContent();
    }


    /*──────── MATCH START ─────*/

    // POST api/rooms/42/start
    [HttpPost("{id:int}/start")]
    public async Task<IActionResult> StartMatch(int id, [FromQuery] int teamId) =>
        Ok(await _svc.RoomService.StartMatchAsync(id, teamId));



    [HttpPost("{roomId:int}/pay")]
    [Authorize]
    public async Task<IActionResult> PayForRoom(int roomId)
    {
        int userId = int.Parse(User.FindFirst("id")!.Value);

        var room = await _svc.RoomService.GetRoomAsync(roomId, userId);
        decimal amount = room.PricePerPlayer;

        await _svc.RoomService.PayPlayerAsync(roomId, userId, amount);
        return NoContent();
    }




    [HttpPost("{id:int}/confirm")]
    public async Task<IActionResult> ConfirmReservation(int id)
    {
        await _svc.RoomService.ConfirmReservationAsync(id);
        return NoContent();
    }
    [HttpGet("payments")]
    public async Task<IActionResult> GetPayments([FromQuery] int ownerId)
    {
        var result = await _svc.RoomService.GetPaymentsByFieldOwnerAsync(ownerId);
        return Ok(result);
    }


    // GET api/rooms/invited?userId=42
    [HttpGet("invited")]
    public async Task<IActionResult> GetRoomsInvitedTo([FromQuery] int userId)
    {
        var rooms = await _svc.RoomService.GetRoomsUserIsInvitedToAsync(userId);
        return Ok(rooms);
    }

}
