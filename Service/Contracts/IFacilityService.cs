using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface IFacilityService
    {
        Task<IEnumerable<FacilityDto>> GetAllFacilitiesAsync(int? facilityId, bool trackChanges);
        Task<FacilityDto> GetFacilityAsync(int FacilityId, bool trackChanges);
        Task<FacilityDto> CreateFacilityAsync(FacilityForCreationDto Facility);
        Task UpdateFacilityPhotos(int reviewerId, int facilityId, FacilityPhotosUpdateDto dto);
        Task UpdateFacilityAsync(int reviewerId, int facilityId, FacilityForUpdateDto facility, bool trackChanges);
        Task PatchFacilityAsync(int reviewerId, int id, FacilityPatchDto patch);
        Task DeleteFacility(int reviewerId, int facilityId, bool trackchanges);
    }
}
