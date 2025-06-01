using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects;

public interface IPhotoService
{
    Task<string> UploadLogoAsync(IFormFile logo, string entityFolder);      // yeni
    Task UploadPhotosAsync(IEnumerable<IFormFile> files,
                           string entityType, int entityId, int? parentId = null);
    Task<IEnumerable<PhotoDto>> GetPhotosAsync(string entityType, int entityId, bool trackChanges);
    Task DeletePhotosByEntityAsync(string entityType, int entityId, bool trackChanges);
}
