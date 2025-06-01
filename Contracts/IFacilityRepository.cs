using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IFacilityRepository
    {
        /* Command */
        void CreateFacility(Facility facility);
        void DeleteFacility(Facility facility);

        /* Query */
        Task<Facility?> GetFacilityAsync(int id, bool trackChanges);
        Task<IEnumerable<Facility>> GetAllFacilitiesAsync(bool trackChanges);
        Task<IEnumerable<Facility>> GetFacilitiesByOwnerIdAsync(int ownerId, bool trackChanges);

        /* Konum tabanlı örnek */
        Task<IEnumerable<Facility>> GetFacilitiesNearAsync(decimal lat, decimal lng, double kmRadius, bool trackChanges);
    }

}