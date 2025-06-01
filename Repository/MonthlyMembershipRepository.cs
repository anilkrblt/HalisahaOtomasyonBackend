using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class MonthlyMembershipRepository
        : RepositoryBase<MonthlyMembership>, IMonthlyMembershipRepository
    {
        public MonthlyMembershipRepository(RepositoryContext ctx) : base(ctx) { }

        public void CreateMonthlyMembership(MonthlyMembership m) => Create(m);
        public void DeleteMonthlyMembership(MonthlyMembership m) => Delete(m);

        public async Task<IEnumerable<MonthlyMembership>> GetAllMonthlyMembershipsAsync(bool trackChanges) =>
            await FindAll(trackChanges)
                  .Include(m => m.Field)
                  .Include(m => m.User)
                  .OrderByDescending(m => m.CreatedAt)
                  .ToListAsync();

        public async Task<MonthlyMembership?> GetOneMonthlyMembershipAsync(int id, bool trackChanges) =>
            await FindByCondition(m => m.Id == id, trackChanges)
                  .Include(m => m.Field)
                  .Include(m => m.User)
                  .SingleOrDefaultAsync();

        public async Task<IEnumerable<MonthlyMembership>> GetMonthlyMembershipsByCustomerIdAsync(int userId, bool trackChanges) =>
            await FindByCondition(m => m.UserId == userId, trackChanges)
                  .Include(m => m.Field)
                  .OrderByDescending(m => m.CreatedAt)
                  .ToListAsync();
    }
}
