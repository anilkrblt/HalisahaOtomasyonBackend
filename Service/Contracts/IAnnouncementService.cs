using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface IAnnouncementService
    {
        Task<IEnumerable<AnnouncementDto>> GetAllAnnouncementsAsync(int? facilityId, bool trackChanges);
        Task<AnnouncementDto> GetAnnouncementAsync(int AnnouncenebtId, bool trackChanges);
        Task<AnnouncementDto> CreateAnnouncementAsync(int facilityId, AnnouncementForCreationDto Announcement);
        Task UpdateAnnouncement(int announcementId, AnnouncementForUpdateDto announcement, bool trackChanges);
        Task DeleteAnnouncement(int announcementId, bool trackchanges);
    }
}
