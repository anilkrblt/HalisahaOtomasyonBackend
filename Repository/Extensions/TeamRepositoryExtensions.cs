using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository.Extensions
{
    public static class TeamRepositoryExtensions
    {
        public static IQueryable<Team> Search(this IQueryable<Team> teams,
            string teamName,
            string city)
        {
            if (!string.IsNullOrWhiteSpace(teamName))
            {
                var lowerTeamName = teamName.Trim().ToLower();
                teams = teams.Where(t => t.Name
                .ToLower()
                .Contains(lowerTeamName));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var lowerCity = city.Trim().ToLower();
                teams = teams.Where(t =>t.City
                .ToLower()
                .Contains(lowerCity));
            }

            return teams;
        }

        public static IQueryable<Team> IncludeMembers(this IQueryable<Team> teams) =>
            teams.Include(t => t.Members).ThenInclude(m => m.User);
    }
}
