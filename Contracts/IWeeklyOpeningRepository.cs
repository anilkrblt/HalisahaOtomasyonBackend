using Entities.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IWeeklyOpeningRepository
    {
        void CreateWeeklyOpening(WeeklyOpening opening);
        void DeleteWeeklyOpening(WeeklyOpening opening);
        Task<WeeklyOpening?> GetWeeklyOpeningAsync(int id, bool trackChanges);
        Task<IEnumerable<WeeklyOpening>> GetWeeklyOpeningsByFieldIdAsync(int fieldId, bool trackChanges);
    }
}