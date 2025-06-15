using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface IEquipmentService
    {
        Task<IEnumerable<EquipmentDto>> GetAllEquipmentsByFacilityAsync(int facilityId, bool trackChanges);
        Task<EquipmentDto> GetEquipmentAsync(int equipmentId, bool trackChanges);
        Task<EquipmentDto> CreateEquipmentAsync(EquipmentForCreationDto equipmentDto, int facilityId);
        Task UpdateEquipmentAsync(int equipmentId, EquipmentForUpdateDto equipmentDto, bool trackChanges);
        Task DeleteEquipmentAsync(int equipmentId, bool trackChanges);
    }
}
