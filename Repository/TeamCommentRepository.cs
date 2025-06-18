using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class TeamCommentRepository
        : RepositoryBase<TeamComment>, ITeamCommentRepository
    {
        public TeamCommentRepository(RepositoryContext context) : base(context) { }
        private static IQueryable<TeamComment> NotDeleted(IQueryable<TeamComment> q) =>
            q.Where(c => !c.IsDeleted); 
        public void CreateTeamComment(TeamComment comment) => Create(comment);
        public void DeleteTeamComment(TeamComment comment) => Delete(comment);

        public async Task<IEnumerable<TeamComment>> GetAllTeamCommentsAsync(bool trackChanges) =>
            await NotDeleted(FindAll(trackChanges))
                 .Include(c => c.FromUser)
                 .OrderByDescending(c => c.CreatedAt)
                 .ToListAsync();

        public async Task<TeamComment?> GetTeamCommentAsync(int commentId, bool trackChanges) =>
            await NotDeleted(
                    FindByCondition(c => c.Id == commentId, trackChanges))
                 .Include(c => c.FromUser)
                 .SingleOrDefaultAsync();

        public async Task<IEnumerable<TeamComment>> GetTeamCommentsByTeamIdAsync(int teamId, bool trackChanges) =>
            await NotDeleted(
                    FindByCondition(c => c.ToTeamId == teamId, trackChanges))
                 .Include(c => c.FromUser)
                 .OrderByDescending(c => c.CreatedAt)
                 .ToListAsync();
    }
}
