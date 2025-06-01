using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class FacilityRatingRepository
        : RepositoryBase<FacilityRating>, IFacilityRatingRepository
    {
        public FacilityRatingRepository(RepositoryContext ctx) : base(ctx) { }

        /* -------- Command -------- */
        public void CreateRating(FacilityRating rating) => Create(rating);
        public void DeleteRating(FacilityRating rating) => Delete(rating);

        /* -------- Query: Tek Kayıt -------- */
        public async Task<FacilityRating?> GetRatingAsync(int facilityId, int userId, bool trackChanges) =>
            await FindByCondition(r => r.FacilityId == facilityId &&
                                       r.UserId     == userId,
                                  trackChanges)
                  .SingleOrDefaultAsync();

        /* -------- Query: Liste -------- */
        public async Task<IEnumerable<FacilityRating>> GetRatingsByFacilityIdAsync(int facilityId, bool trackChanges) =>
            await FindByCondition(r => r.FacilityId == facilityId, trackChanges)
                  .Include(r => r.User)
                  .OrderByDescending(r => r.CreatedAt)
                  .ToListAsync();

        public async Task<IEnumerable<FacilityRating>> GetRatingsByUserIdAsync(int userId, bool trackChanges) =>
            await FindByCondition(r => r.UserId == userId, trackChanges)
                  .Include(r => r.Facility)
                  .OrderByDescending(r => r.CreatedAt)
                  .ToListAsync();

        /* -------- İsteğe Bağlı: Ortalama -------- */
        public async Task<double> GetAverageStarsForFacilityAsync(int facilityId) =>
            await RepositoryContext.FacilityRatings
                .Where(r => r.FacilityId == facilityId)
                .AverageAsync(r => (double?)r.Stars) ?? 0.0;
    }
}
