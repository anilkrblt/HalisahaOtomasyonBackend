using Entities.Models;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IFacilityService
    {
        Task DeleteFacility(int facilityId, bool trackchanges);

        Task<FacilityDto> CreateFacilityAsync(FacilityForCreationDto Facility);

        Task<IEnumerable<FacilityDto>> GetAllFacilitiesAsync(bool trackChanges);

        Task<FacilityDto> GetFacilityAsync(int FacilityId, bool trackChanges);
        Task UpdateFacilityAsync(int facilityId, FacilityForUpdateDto facility, bool trackChanges);

        Task PatchFacilityAsync(int id, FacilityPatchDto patch);


    }
}
