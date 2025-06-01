using Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IFacilityRatingRepository
    {
        /* Command */
        void CreateRating(FacilityRating rating);
        void DeleteRating(FacilityRating rating);

        /* Query — tek kayıt (bileşik PK) */
        Task<FacilityRating?> GetRatingAsync(int facilityId, int userId, bool trackChanges);

        /* Query — listeler */
        Task<IEnumerable<FacilityRating>> GetRatingsByFacilityIdAsync(int facilityId, bool trackChanges);
        Task<IEnumerable<FacilityRating>> GetRatingsByUserIdAsync(int userId, bool trackChanges);

        /* İsteğe bağlı: ortalama puan */
        Task<double> GetAverageStarsForFacilityAsync(int facilityId);
    }
}
