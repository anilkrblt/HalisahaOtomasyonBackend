// Repository/FieldExceptionRepository.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class FieldExceptionRepository
        : RepositoryBase<FieldException>, IFieldExceptionRepository
    {
        public FieldExceptionRepository(RepositoryContext repositoryContext)
            : base(repositoryContext) { }

        public void CreateFieldException(FieldException exception)
            => Create(exception);

        public void DeleteFieldException(FieldException exception)
            => Delete(exception);

        public async Task<FieldException?> GetFieldExceptionAsync(int id, bool trackChanges)
            => await FindByCondition(e => e.Id == id, trackChanges)
                          .SingleOrDefaultAsync();

        public async Task<IEnumerable<FieldException>> GetExceptionsByFieldIdAsync(
            int fieldId, bool trackChanges)
            => await FindByCondition(e => e.FieldId == fieldId, trackChanges)
                      .ToListAsync();

        public async Task<FieldException?> GetExceptionByDateAsync(
            int fieldId, DateTime date, bool trackChanges)
            => await FindByCondition(e =>
                       e.FieldId == fieldId && e.Date.Date == date.Date,
                       trackChanges)
                      .SingleOrDefaultAsync();

        public void DeleteFieldExceptions(IEnumerable<FieldException> exceptions)
        {
            RepositoryContext.FieldExceptions.RemoveRange(exceptions);
        }

        

    }
}
