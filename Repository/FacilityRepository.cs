using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class FacilityRepository : RepositoryBase<Facility>, IFacilityRepository
    {
        public FacilityRepository(RepositoryContext ctx) : base(ctx) { }

        public void CreateFacility(Facility f) => Create(f);
        public void DeleteFacility(Facility f) => Delete(f);

        public async Task<IEnumerable<Facility>> GetAllFacilitiesAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                    .Include(f => f.Photos)
                    .Include(f => f.Equipments)
                    .Include(f => f.Owner)
                    .Include(f => f.Fields)
                    .OrderBy(f => f.Name)
                    .ToListAsync();

        public async Task<Facility?> GetFacilityAsync(int id, bool trackChanges) =>
            await FindByCondition(f => f.Id == id, trackChanges)
                  .Include(f => f.Fields)
                  .Include(f => f.Ratings)
                      .Include(f => f.Photos)
                    .Include(f => f.Equipments)
                  .SingleOrDefaultAsync();

        public async Task<IEnumerable<Facility>> GetFacilitiesByOwnerIdAsync(int ownerId, bool trackChanges) =>
            await FindByCondition(f => f.OwnerId == ownerId, trackChanges)
                    .Include(f => f.Photos)
                    .Include(f => f.Equipments)
                    .Include(f => f.Fields)
                     .OrderBy(f => f.Name)

                     .ToListAsync();

        /* Basit harversine hesaplı yakınlık sorgusu (server-side) */
        public async Task<IEnumerable<Facility>> GetFacilitiesNearAsync(
     decimal lat, decimal lng, double kmRadius, bool trackChanges)
        {
            const double R = 6371.0;                      // Dünya yarıçapı (km)
            double Deg2Rad(double d) => d * Math.PI / 180;

            /* 1) Uygun konumu olan tüm tesisleri DB’den çek */
            var list = await FindAll(trackChanges)
                .Where(f => f.Latitude.HasValue && f.Longitude.HasValue)
                .ToListAsync();                           // <-- hâlâ IQueryable, o yüzden OK

            /* 2) Bellekte harversine hesabı */
            var latRad = Deg2Rad((double)lat);
            var lngRad = Deg2Rad((double)lng);

            return list.Where(f =>
            {
                var dLat = Deg2Rad((double)(f.Latitude! - lat));
                var dLng = Deg2Rad((double)(f.Longitude! - lng));
                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(latRad) * Math.Cos(Deg2Rad((double)f.Latitude!)) *
                        Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                return R * c <= kmRadius;
            }).ToList();
        }

    }

}