using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IAnnouncementService
    {

        Task DeleteAnnouncement(int announcementId, bool trackchanges);

        Task<AnnouncementDto> CreateAnnouncementAsync(int facilityId, AnnouncementForCreationDto Announcement);

        Task<IEnumerable<AnnouncementDto>> GetAllAnnouncementsAsync(bool trackChanges);

        Task<AnnouncementDto> GetAnnouncementAsync(int AnnouncenebtId, bool trackChanges);
        Task UpdateAnnouncement(int announcementId, AnnouncementForUpdateDto announcement, bool trackChanges);

    }
}
