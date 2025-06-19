using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers
{
    // TODO düzenle
    // bildirimleri otomatikleştir ve sadece get ve belki put istekleri kalsın
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly IServiceManager _service;

        public NotificationsController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifs = await _service.NotificationService.GetAllNotificationsAsync();
            return Ok(notifs);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetNotification(int id)
        {
            var notif = await _service.NotificationService.GetNotificationAsync(id);
            return Ok(notif);
        }

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var notifs = await _service.NotificationService.GetNotificationsByUserIdAsync(userId);
            return Ok(notifs);
        }

        [HttpGet("facility/{facilityId:int}")]
        public async Task<IActionResult> GetByFacilityId(int facilityId)
        {
            var notifs = await _service.NotificationService.GetNotificationsByFacilityIdAsync(facilityId);
            return Ok(notifs);
        }

        [HttpGet("user/{userId:int}/unread")]
        public async Task<IActionResult> GetUnread(int userId)
        {
            var notifs = await _service.NotificationService.GetUnreadNotificationsAsync(userId);
            return Ok(notifs);
        }



        [HttpPut("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _service.NotificationService.MarkAsReadAsync(id);
            return NoContent();
        }
    }
}
