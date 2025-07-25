using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Shared.DataTransferObjects;

namespace Service
{
    public class PhotoService : IPhotoService
    {
        private readonly IRepositoryManager _repo;
        private readonly IMapper _map;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] _allowed = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
        private const int _maxSize = 5 * 1024 * 1024;

        public PhotoService(IRepositoryManager repo,
                            IMapper map,
                            IWebHostEnvironment env)
        {
            _repo = repo;
            _map = map;
            _env = env;
        }

        private string GetWebRoot()
        {
            var root = _env.WebRootPath
                       ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            Directory.CreateDirectory(root);         
            return root;
        }

        private static void ValidateFile(IFormFile f)
        {
            var ext = Path.GetExtension(f.FileName).ToLower();
            if (!_allowed.Contains(ext))
                throw new InvalidOperationException($"Geçersiz format: {ext}");
            if (f.Length > _maxSize)
                throw new InvalidOperationException("Dosya 5 MB’tan büyük.");
        }

        public async Task<string> UploadLogoAsync(IFormFile file, string entityFolder)
        {
            if (string.IsNullOrWhiteSpace(entityFolder))
                throw new ArgumentException("entityFolder boş olamaz.", nameof(entityFolder));

            ValidateFile(file);

            var ext = Path.GetExtension(file.FileName).ToLower();
            var webRoot = GetWebRoot();
            var folder = Path.Combine(webRoot, "images", entityFolder);
            Directory.CreateDirectory(folder);

            var logoName = $"logo{ext}";
            var absPath = Path.Combine(folder, logoName);

            await using (var fs = new FileStream(absPath, FileMode.Create))
                await file.CopyToAsync(fs);

            return $"/images/{entityFolder}/{logoName}";
        }

        public async Task UploadPhotosAsync(IEnumerable<IFormFile> files,
                                            string entityType,
                                            int entityId,
                                            int? parentId = null)
        {
            var folderRel = entityType.ToLower() == "field" && parentId is not null
                ? Path.Combine("facility", parentId.Value.ToString(), "field", entityId.ToString())
                : Path.Combine(entityType.ToLower(), entityId.ToString());

            var webRoot = GetWebRoot();
            var folder = Path.Combine(webRoot, "images", folderRel);
            Directory.CreateDirectory(folder);

            foreach (var file in files.Take(5))
            {
                ValidateFile(file);

                var ext = Path.GetExtension(file.FileName).ToLower();
                var fname = $"{Path.GetFileNameWithoutExtension(file.FileName)
                               .Replace(' ', '_')}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
                var abs = Path.Combine(folder, fname);

                await using (var stream = new FileStream(abs, FileMode.Create))
                    await file.CopyToAsync(stream);

                var rel = $"/images/{folderRel.Replace('\\', '/')}/{fname}";

                _repo.Photo.CreatePhoto(new Photo
                {
                    EntityType = entityType.ToLower(),
                    EntityId = entityId,
                    Url = rel
                });
            }

            await _repo.SaveAsync();
        }

        public async Task<IEnumerable<PhotoDto>> GetPhotosAsync(string entityType, int entityId, bool track) =>
            _map.Map<IEnumerable<PhotoDto>>(
                await _repo.Photo.GetAllByEntityAsync(entityType.ToLower(), entityId, track));

        public async Task DeletePhotosByEntityAsync(string entityType, int entityId, bool track)
        {
            var rows = await _repo.Photo.GetAllByEntityAsync(entityType, entityId, track);

            var webRoot = GetWebRoot();
            var relFolder = Path.Combine(entityType.ToLower(), entityId.ToString());
            var absFolder = Path.Combine(webRoot, "images", relFolder);

            if (Directory.Exists(absFolder))
                Directory.Delete(absFolder, true);

            foreach (var p in rows)
                _repo.Photo.DeletePhoto(p);

            await _repo.SaveAsync();
        }
    }
}
