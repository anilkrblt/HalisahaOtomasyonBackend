using Entities.Models;

namespace Contracts
{
    public interface IAnnouncementRepository
    {
        Task<IEnumerable<Announcement>> GetAnnouncementsAsync(bool trackChanges);
        Task<IEnumerable<Announcement>> GetAnnouncementsForFacilityAsync(int facilityId, bool trackChanges);
        Task<Announcement> GetAnnouncementAsync(int AnnouncementId, bool trackChanges);
        void CreateAnnouncement(Announcement Announcement);
        void DeleteAnnouncement(Announcement Announcement);
    }
}
