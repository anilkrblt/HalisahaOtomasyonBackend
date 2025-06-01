using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IMonthlyMembershipRepository
    {
        void CreateMonthlyMembership(MonthlyMembership membership);
        void DeleteMonthlyMembership(MonthlyMembership membership);

        Task<IEnumerable<MonthlyMembership>> GetAllMonthlyMembershipsAsync(bool trackChanges);
        Task<MonthlyMembership?> GetOneMonthlyMembershipAsync(int id, bool trackChanges);
        Task<IEnumerable<MonthlyMembership>> GetMonthlyMembershipsByCustomerIdAsync(int userId, bool trackChanges);
    }

}