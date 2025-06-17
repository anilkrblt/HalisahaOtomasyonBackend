// Presentation/Controllers/FriendshipsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/friendships")]
    // TODO bu controlleri ve servisi kontrol et bi temizlik yap
    // TODO arkadaşlık isteği iptal kısmında bug var
    public class FriendshipsController : ControllerBase
    {
        private readonly IServiceManager _svc;
        public FriendshipsController(IServiceManager svc)
        {
            _svc = svc;
        }


        private int CallerId =>
            int.Parse(User.FindFirst("id")?.Value);



        [HttpGet("users/{userId:int}")]
        public async Task<IActionResult> GetFriends(int userId)
        {
            var list = await _svc.FriendshipService.GetFriendsAsync(userId, false);
            return Ok(list);
        }


        [HttpGet("users/{userId:int}/pending")]
        public async Task<IActionResult> GetPending(int userId)
        {
            var list = await _svc.FriendshipService.GetPendingRequestsAsync(userId, false);
            return Ok(list);
        }

        [HttpGet("users/{userId:int}/outgoing")]
        public async Task<IActionResult> GetOutgoingRequests(int userId)
        {
            if (CallerId != userId)
                return Forbid();

            var list = await _svc.FriendshipService.GetOutgoingRequestsAsync(userId, false);
            return Ok(list);
        }



        [HttpPost]
        public async Task<IActionResult> SendRequest([FromBody] FriendRequestForCreationDto dto)
        {

            var created = await _svc.FriendshipService.SendFriendRequestAsync(CallerId, dto.ToUserId);
            return Created(string.Empty, created);
        }

        /*──── İstek Yanıtla ────*/
        [HttpPut("{userA:int}/{userB:int}")]
        public async Task<IActionResult> Respond(int userA, int userB,
                                                 [FromBody] FriendRequestRespondDto dto)
        {
            if (CallerId != userA && CallerId != userB)
                return Forbid();

            await _svc.FriendshipService.RespondFriendRequestAsync(userB, userA, dto.Status);
            return NoContent();
        }

        /*──── Arkadaşlığı Sil ────*/
        [HttpDelete("{userA:int}/{userB:int}")]
        public async Task<IActionResult> Delete(int userA, int userB)
        {
            if (CallerId != userA && CallerId != userB)
                return Forbid();

            await _svc.FriendshipService.DeleteFriendAsync(userA, userB);
            return NoContent();
        }

        /*──── Kullanıcı adı arama (autocomplete) ────*/
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int take = 10)
        {
            var users = await _svc.FriendshipService.SearchCustomersAsync(q, take);

            var results = new List<object>();
            foreach (var c in users)
            {
                // Her kullanıcı için fotoğrafı sırayla çek (paralel değil!)
                var photos = await _svc.PhotoService.GetPhotosAsync("user", c.Id, false);
                var photoUrl = photos.FirstOrDefault()?.Url;
                results.Add(new
                {
                    c.Id,
                    c.UserName,
                    c.FullName,
                    PhotoUrl = photoUrl
                });
            }

            return Ok(results);
        }


        // TODO sıkıntı
        /*──── Bekleyen isteği iptal ────*/
        [HttpDelete("{fromId:int}/{toId:int}/cancel")]
        public async Task<IActionResult> Cancel(int fromId, int toId)
        {
            if (CallerId != fromId)
                return Forbid();

            await _svc.FriendshipService.CancelFriendRequestAsync(fromId, toId);
            return NoContent();
        }
    }
}
