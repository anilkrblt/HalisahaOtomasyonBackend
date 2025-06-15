using Entities.Models;

namespace Contracts
{
    public interface IEquipmentRepository
    {
        Task<IEnumerable<Equipment>> GetEquipmentsByFacilityIdAsync(int facilityId, bool trackChanges);
        Task<Equipment> GetEquipmentByIdAsync(int id, bool trackChanges);
        void CreateEquipment(Equipment equipment);
        void DeleteEquipment(Equipment equipment);
    }
}