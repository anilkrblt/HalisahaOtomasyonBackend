using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class FieldRepository : RepositoryBase<Field>, IFieldRepository
    {
        public FieldRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public void CreateField(Field field) => Create(field);

        public void DeleteField(Field field) => Delete(field);

        public async Task<IEnumerable<Field>> GetAllFieldsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                  .Include(f => f.Facility)
                  .Include(f => f.Rooms)
                  .Include(f => f.WeeklyOpenings)
                  .Include(f => f.Exceptions)
                  .Include(f => f.Comments)
                  .OrderBy(f => f.Name)
                  .ToListAsync();


        public async Task<IEnumerable<Field>> GetFieldsByFacilityIdAsync(int facilityId, bool trackChanges) =>
         await FindByCondition(f => f.FacilityId.Equals(facilityId), trackChanges)
                .Include(f => f.Rooms)
                .Include(f => f.Comments)
                .ToListAsync();

        public async Task<Field?> GetFieldAsync(int fieldId, bool trackChanges) =>
             await FindByCondition(f => f.Id == fieldId, trackChanges)
                  .Include(f => f.Facility)
                  .Include(f => f.Rooms)
                  .Include(f => f.WeeklyOpenings)
                  .Include(f => f.Exceptions)
                  .Include(f => f.Comments)
                  .SingleOrDefaultAsync();

        public async Task<int> GetFacilityIdByFieldIdAsync(int fieldId)
        {
            var field = await GetFieldAsync(fieldId, trackChanges: false);
            return field.FacilityId;
        }


    }
}