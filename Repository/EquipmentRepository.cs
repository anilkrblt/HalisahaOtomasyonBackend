using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class EquipmentRepository : RepositoryBase<Equipment>, IEquipmentRepository
    {
        public EquipmentRepository(RepositoryContext ctx) : base(ctx) { }

        public void CreateEquipment(Equipment equipment) => Create(equipment);
        public void DeleteEquipment(Equipment equipment) => Delete(equipment);

        public async Task<IEnumerable<Equipment>> GetAllByFacilityIdAsync(int facilityId, bool trackChanges) =>
            await FindByCondition(e => e.FacilityId == facilityId, trackChanges)
                  .OrderBy(e => e.Name)
                  .ToListAsync();

        public async Task<Equipment?> GetEquipmentByIdAsync(int id, bool trackChanges) =>
            await FindByCondition(e => e.Id == id, trackChanges)
                  .SingleOrDefaultAsync();
    }

}