// Service.Contracts/IFacilityRatingService.cs
using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts;

public interface IFacilityRatingService
{
    Task<IEnumerable<FacilityRatingDto>> GetRatingsByFacilityAsync(int facilityId, bool trackChanges);
    Task<double> GetAverageStarsAsync(int facilityId);
    Task<FacilityRatingDto> AddRatingAsync(int facilityId, FacilityRatingForCreationDto dto, int userId);
    Task UpdateRatingAsync(int facilityId, int userId, FacilityRatingForUpdateDto dto);
    Task<IEnumerable<FacilityDto>> GetRatedFacilitiesByUserAsync(int userId, bool trackChanges);


}
