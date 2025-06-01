using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

// Bir halısaha sahibi duyuru oluşturdugunda kendi duyuru sayfasında görünsün
// 

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
                .Include(a => a.Facility) // Announcement -> Facility ilişkisini getiriyoruz
                .SingleOrDefaultAsync();

        public async Task<IEnumerable<Announcement>> GetAnnouncementsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                .Include(a => a.Facility)
                .OrderBy(a => a.Id)
                .ToListAsync();
    }
}
