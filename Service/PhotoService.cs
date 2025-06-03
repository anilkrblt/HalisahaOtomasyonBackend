// Service/PhotoService.cs
using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Service
{
    public class PhotoService : IPhotoService
    {
        private readonly IRepositoryManager _repo;
        private readonly IMapper _map;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] _allowed = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
        private const int _maxSize = 5 * 1024 * 1024; // 5 MB

        public PhotoService(
            IRepositoryManager repo,
            IMapper map,
            IWebHostEnvironment env
        )
        {
            _repo = repo;
            _map = map;
            _env = env;
        }

        /*────────────────── PUBLIC API ──────────────────*/

        /// <summary>
        /// 1) Logo – tek dosya, URL geri döner
        /// </summary>
        public async Task<string> UploadLogoAsync(IFormFile file, string entityFolder)
        {
            if (string.IsNullOrWhiteSpace(entityFolder))
                throw new ArgumentException("entityFolder boş olamaz.", nameof(entityFolder));
            ValidateFile(file);

            var ext = Path.GetExtension(file.FileName).ToLower();
            // Fiziksel yol: wwwroot/images/{entityFolder}
            var folderAbs = Path.Combine(_env.WebRootPath, "images", entityFolder);
            Directory.CreateDirectory(folderAbs);

            var logoName = $"logo{ext}";
            var absPath = Path.Combine(folderAbs, logoName);
            await using var fs = new FileStream(absPath, FileMode.Create);
            await file.CopyToAsync(fs);

            // Tarayıcıdan erişim için görece URL
            return $"/images/{entityFolder}/{logoName}";
        }

        /// <summary>
        /// 2) Çoklu fotoğraf
        /// </summary>
        public async Task UploadPhotosAsync(
            IEnumerable<IFormFile> files,
            string entityType,
            int entityId,
            int? parentId = null
        )
        {
            // Eğer field altındaki facility’ye bağlı bir fotoğrafsa dizin: facility/{parentId}/field/{entityId}
            var folderRel = entityType.ToLower() == "field" && parentId is not null
                ? Path.Combine("facility", parentId.Value.ToString(), "field", entityId.ToString())
                : Path.Combine(entityType.ToLower(), entityId.ToString());

            // Fiziksel yol: wwwroot/images/{folderRel}
            var folderAbs = Path.Combine(_env.WebRootPath, "images", folderRel);
            Directory.CreateDirectory(folderAbs);

            foreach (var file in files.Take(5))
            {
                ValidateFile(file);

                var ext = Path.GetExtension(file.FileName).ToLower();
                var fname = $"{Path.GetFileNameWithoutExtension(file.FileName)
                               .Replace(' ', '_')}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
                var abs = Path.Combine(folderAbs, fname);

                await using var stream = new FileStream(abs, FileMode.Create);
                await file.CopyToAsync(stream);

                // Görece URL: /images/{folderRel}/{fname}
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

        /// <summary>
        /// 3) Listeleme
        /// </summary>
        public async Task<IEnumerable<PhotoDto>> GetPhotosAsync(string entityType, int entityId, bool track) =>
            _map.Map<IEnumerable<PhotoDto>>(
                await _repo.Photo.GetAllByEntityAsync(entityType.ToLower(), entityId, track));

        /// <summary>
        /// 4) Silme (fotoğraflar + logo dâhil)
        /// </summary>
        public async Task DeletePhotosByEntityAsync(string entityType, int entityId, bool track)
        {
            var rows = await _repo.Photo.GetAllByEntityAsync(entityType, entityId, track);

            // Görece klasör: images/{entityType}/{entityId}
            var relFolder = Path.Combine(entityType.ToLower(), entityId.ToString());
            var absFolder = Path.Combine(_env.WebRootPath, "images", relFolder);
            if (Directory.Exists(absFolder))
                Directory.Delete(absFolder, true);

            foreach (var p in rows)
                _repo.Photo.DeletePhoto(p);

            await _repo.SaveAsync();
        }

        /*────────────────── Yardımcılar ──────────────────*/

        private static void ValidateFile(IFormFile f)
        {
            var ext = Path.GetExtension(f.FileName).ToLower();
            if (!_allowed.Contains(ext))
                throw new InvalidOperationException($"Geçersiz format: {ext}");
            if (f.Length > _maxSize)
                throw new InvalidOperationException("Dosya 5 MB’tan büyük.");
        }
    }
}
