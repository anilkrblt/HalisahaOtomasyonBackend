using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IFieldRepository
    {
        void DeleteField(Field field);

        void CreateField(Field field);

        Task<IEnumerable<Field>> GetAllFieldsAsync(bool trackChanges);
        Task<Field> GetFieldAsync(int fieldId, bool trackChanges);
        Task<IEnumerable<Field>> GetFieldsByFacilityIdAsync(int facilityId, bool trackChanges);
        Task<int> GetFacilityIdByFieldIdAsync(int fieldId);
        Task<IEnumerable<Field>> GetFieldsByOwnerIdAsync(int ownerId, bool trackChanges);


    }
}