using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAnnouncementRepository
    {
        void DeleteAnnouncement(Announcement Announcement);

        void CreateAnnouncement(Announcement Announcement);

        Task<IEnumerable<Announcement>> GetAnnouncementsAsync(bool trackChanges);

        Task<Announcement> GetAnnouncementAsync(int AnnouncementId, bool trackChanges);

    }
}
