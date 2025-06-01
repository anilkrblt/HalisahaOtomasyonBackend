using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IPhotoRepository
    {
        Task<Photo> GetPhotoAsync(int photoId, bool trackChanges);
        Task<IEnumerable<Photo>> GetAllByEntityAsync(string entityType, int entityId, bool trackChanges);
        void CreatePhoto(Photo photo);
        void DeletePhoto(Photo photo);
    }

}