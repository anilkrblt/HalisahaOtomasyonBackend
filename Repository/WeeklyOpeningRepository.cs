// Repository/WeeklyOpeningRepository.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class WeeklyOpeningRepository 
        : RepositoryBase<WeeklyOpening>, IWeeklyOpeningRepository
    {
        public WeeklyOpeningRepository(RepositoryContext repositoryContext) 
            : base(repositoryContext) { }

        public void CreateWeeklyOpening(WeeklyOpening opening) 
            => Create(opening);

        public void DeleteWeeklyOpening(WeeklyOpening opening) 
            => Delete(opening);

        public async Task<WeeklyOpening?> GetWeeklyOpeningAsync(int id, bool trackChanges) 
            => await FindByCondition(w => w.Id == id, trackChanges)
                          .SingleOrDefaultAsync();

        public async Task<IEnumerable<WeeklyOpening>> GetWeeklyOpeningsByFieldIdAsync(
            int fieldId, bool trackChanges)
            => await FindByCondition(w => w.FieldId == fieldId, trackChanges)
                      .ToListAsync();
    }
}
