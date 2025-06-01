using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Repository.Extensions
{


    // using Repository.Extensions; => 
    public static class RepositoryFacilityExtensions
    {
        public static IQueryable<Facility> FilterFacilities(this IQueryable<Facility>
       facilities, uint minAge, uint maxAge) =>
        facilities.Where(e => e.OwnerId == minAge/*(e.Age >= minAge && e.Age <= maxAge)*/);

        public static IQueryable<Facility> Search(this IQueryable<Facility> facilities, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return facilities;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return facilities.Where(e => e.Name.ToLower().Contains(lowerCaseTerm));
        }
    }

}