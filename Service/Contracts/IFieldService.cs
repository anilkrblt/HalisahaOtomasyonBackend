using Entities.Models;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IFieldService
    {
        Task<IEnumerable<FieldDto>> GetAllFieldsAsync(bool trackChanges);
        Task<FieldDto> GetFieldAsync(int fieldId, bool trackChanges);
        Task<IEnumerable<FieldDto>> GetFieldsByFacilityIdAsync(int facilityId, bool trackChanges);
        Task<FieldDto> CreateFieldAsync(FieldForCreationDto field);
        Task UpdateFieldAsync(int fieldId, FieldForUpdateDto field, bool trackChanges);
        Task DeleteFieldAsync(int fieldId);

        Task<bool> IsFieldOpenAsync(int fieldId, DateTime dateTime);

    }
}
