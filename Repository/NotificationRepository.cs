using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(RepositoryContext repositoryContext) : base(repositoryContext) { }

        public void CreateNotification(Notification notification) => Create(notification);

        public void DeleteNotification(Notification notification) => Delete(notification);

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<Notification> GetOneNotificationAsync(int notificationId, bool trackChanges) =>
            await FindByCondition(n => n.Id == notificationId, trackChanges)
                .Include(n => n.User)
                .Include(n => n.Facility)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Notification>> GetNotificationsByCustomerIdAsync(int customerId, bool trackChanges) =>
            await FindByCondition(n => n.UserId == customerId, trackChanges)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Notification>> GetNotificationsByFacilityIdAsync(int facilityId, bool trackChanges) =>
            await FindByCondition(n => n.FacilityId == facilityId, trackChanges)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(int userId) =>
            await FindByCondition(n => n.UserId == userId && !n.IsRead, trackChanges: false)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await FindByCondition(n => n.Id == notificationId, true).FirstOrDefaultAsync();
            if (notification is not null)
            {
                notification.IsRead = true;
            }
        }
    }
}
