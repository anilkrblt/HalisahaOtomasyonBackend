// Presentation/Controllers/FriendshipsController.cs
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/friendships")]
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


        [HttpGet("users/mobil/{userId:int}")]
        public async Task<IActionResult> GetFriendsFixedFormat(int userId)
        {
            var list = await _svc.FriendshipService.GetFriendsMobilAsync(userId, false);
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
            await _svc.NotificationService.CreateNotificationAsync(new NotificationForCreationDto
            {
                UserId = dto.ToUserId,
                Title = "Yeni Arkadaşlık İsteği",
                Content = "Bir kullanıcı sana arkadaşlık isteği gönderdi.",
                RelatedId = CallerId,
                RelatedType = "user"
            });
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

            if (dto.Status == FriendshipStatus.Accepted)
            {
                int otherUserId = CallerId == userA ? userB : userA;

                await _svc.NotificationService.CreateNotificationAsync(new NotificationForCreationDto
                {
                    UserId = otherUserId,
                    Title = "Arkadaşlık İsteği Kabul Edildi",
                    Content = "Arkadaşlık isteğiniz kabul edildi.",
                    RelatedId = CallerId,
                    RelatedType = "user"
                });
            }
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

        /*──── Kullanıcı adı arama ────*/
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int take = 10)
        {
            var users = await _svc.FriendshipService.SearchCustomersAsync(q, take);

            var results = new List<object>();
            foreach (var c in users)
            {
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
