using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TeamCommentRepository
        : RepositoryBase<TeamComment>, ITeamCommentRepository
    {
        public TeamCommentRepository(RepositoryContext context) : base(context) { }

        /* -------------------- Yardımcı filtre -------------------- */
        private static IQueryable<TeamComment> NotDeleted(IQueryable<TeamComment> q) =>
            q.Where(c => !c.IsDeleted);           // soft-delete filtresi

        /* -------------------- CREATE / DELETE ------------------- */
        public void CreateTeamComment(TeamComment comment) => Create(comment);
        public void DeleteTeamComment(TeamComment comment) => Delete(comment);

        /* -------------------- READ ------------------------------ */
        public async Task<IEnumerable<TeamComment>> GetAllTeamCommentsAsync(bool trackChanges) =>
            await NotDeleted(FindAll(trackChanges))
                 .OrderByDescending(c => c.CreatedAt)
                 .ToListAsync();

        public async Task<TeamComment?> GetTeamCommentAsync(int commentId, bool trackChanges) =>
            await NotDeleted(
                    FindByCondition(c => c.Id == commentId, trackChanges))
                 .SingleOrDefaultAsync();

        public async Task<IEnumerable<TeamComment>> GetTeamCommentsByTeamIdAsync(int teamId, bool trackChanges) =>
            await NotDeleted(
                    FindByCondition(c => c.ToTeamId == teamId, trackChanges))
                 .OrderByDescending(c => c.CreatedAt)
                 .ToListAsync();
    }
}
