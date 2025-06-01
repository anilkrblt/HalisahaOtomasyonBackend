using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{

    public interface IEquipmentRepository
    {
        Task<IEnumerable<Equipment>> GetAllByFacilityIdAsync(int facilityId, bool trackChanges);
        Task<Equipment?> GetEquipmentByIdAsync(int id, bool trackChanges);

        void CreateEquipment(Equipment equipment);
        void DeleteEquipment(Equipment equipment);
    }

}