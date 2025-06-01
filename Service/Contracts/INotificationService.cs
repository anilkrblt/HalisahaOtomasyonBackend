using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync();
        Task<NotificationDto> GetNotificationAsync(int notificationId);
        Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(int userId);
        Task<IEnumerable<NotificationDto>> GetNotificationsByFacilityIdAsync(int facilityId);
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
        Task<NotificationDto> CreateNotificationAsync(NotificationForCreationDto dto);
        Task DeleteNotificationAsync(int notificationId);
    }
}
