using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface INotificationRepository
    {
        void DeleteNotification(Notification Notification);
        void CreateNotification(Notification Notification);
        Task<IEnumerable<Notification>> GetAllNotificationsAsync(bool trackChanges);
        Task<Notification> GetOneNotificationAsync(int NotificationId, bool trackChanges);
        Task<IEnumerable<Notification>> GetNotificationsByCustomerIdAsync(int customerId, bool trackChanges);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId);
        Task<IEnumerable<Notification>> GetNotificationsByFacilityIdAsync(int facilityId, bool trackChanges);
        Task MarkAsReadAsync(int notificationId);


    }
}