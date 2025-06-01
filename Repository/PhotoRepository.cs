using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class PhotoRepository : RepositoryBase<Photo>, IPhotoRepository
    {
        public PhotoRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }

        public void CreatePhoto(Photo photo) => Create(photo);

        public void DeletePhoto(Photo photo) => Delete(photo);

        public async Task<IEnumerable<Photo>> GetAllByEntityAsync(string entityType, int entityId, bool trackChanges) =>
        await FindByCondition(p => p.EntityType == entityType && p.EntityId == entityId, trackChanges)
        .ToListAsync();

        public async Task<Photo> GetPhotoAsync(int photoId, bool trackChanges) =>
        await FindByCondition(p => p.Id.Equals(photoId), trackChanges).SingleOrDefaultAsync();
    }
}