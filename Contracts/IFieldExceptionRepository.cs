using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IFieldExceptionRepository
    {
        void CreateFieldException(FieldException exception);
        void DeleteFieldException(FieldException exception);
        Task<FieldException?> GetFieldExceptionAsync(int id, bool trackChanges);
        Task<IEnumerable<FieldException>> GetExceptionsByFieldIdAsync(int fieldId, bool trackChanges);
        Task<FieldException?> GetExceptionByDateAsync(int fieldId, DateTime date, bool trackChanges);
        void DeleteFieldExceptions(IEnumerable<FieldException> exceptions);

    }
}