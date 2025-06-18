using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class AnnouncementRepository : RepositoryBase<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateAnnouncement(Announcement announcement) => Create(announcement);

        public void DeleteAnnouncement(Announcement announcement) => Delete(announcement);

        public async Task<Announcement?> GetAnnouncementAsync(int announcementId, bool trackChanges) =>
            await FindByCondition(a => a.Id == announcementId, trackChanges)
                .Include(a => a.Facility)
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Announcement>> GetAnnouncementsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .Include(a => a.Facility)
                .OrderBy(a => a.Id)
                .ToListAsync();

        public async Task<IEnumerable<Announcement>> GetAnnouncementsForFacilityAsync(int facilityId, bool trackChanges) =>
            await FindByCondition(a => a.FacilityId == facilityId, trackChanges)
                .Include(a => a.Facility)
                .OrderBy(a => a.Id)
                .ToListAsync();
    }
}
